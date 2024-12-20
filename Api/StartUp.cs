using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infra.Db;

public class StartUp 
{
    public IConfiguration Configuration { get; set; }
    private string key;
    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration.GetSection("Jwt").ToString();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
        }).AddJwtBearer(option => {
            option.TokenValidationParameters = new TokenValidationParameters {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdminServico, AdminServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Token Bearer {token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement 
            {
                {
                    new OpenApiSecurityScheme 
                    {
                        Reference = new OpenApiReference 
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        services.AddDbContext<DbContexto>(options =>
            options.UseNpgsql(
                Configuration.GetConnectionString("ConexaoPadrao")
            )
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
    {
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();

        app.UseEndpoints(endpoints => {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Admin 
            string GerarToken(Admin admin) {
                if (string.IsNullOrWhiteSpace(key)) return string.Empty;
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>() {
                    new Claim ("Email", admin.Email),
                    new Claim ("Perfil", admin.Perfil),
                    new Claim (ClaimTypes.Role, admin.Perfil)
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("admin/login", ([FromBody] LoginDTO loginDTO, IAdminServico adminServico) => {
                var login = adminServico.Login(loginDTO);
                if (login != null) {
                    string token = GerarToken(login);
                    return Results.Ok(new AdminLogado {
                        Email = login.Email,
                        Perfil = login.Perfil,
                        Token = token
                    });
                }
                else {
                    return Results.Unauthorized();
                }
            })
            .WithTags("Admin")
            .AllowAnonymous();

            endpoints.MapPost("admin", ([FromBody] AdminDTO adminDTO, IAdminServico adminServico) => {
                var validacoes = new ErrosDeValidacao{
                    Mensagens = new List<string>()
                };

                if (string.IsNullOrWhiteSpace(adminDTO.Email))
                    validacoes.Mensagens.Add("Email não pode ficar em branco");

                if (string.IsNullOrWhiteSpace(adminDTO.Senha))
                    validacoes.Mensagens.Add("Senha não pode ficar em branco");
                
                if (adminDTO.Perfil == null)
                    validacoes.Mensagens.Add("Perfil não pode ficar em branco");

                if (validacoes.Mensagens.Count() > 0) 
                    return Results.BadRequest(validacoes);

                var admin = new Admin {
                    Email = adminDTO.Email,
                    Senha = adminDTO.Senha,
                    Perfil = adminDTO.Perfil.ToString()
                };
                
                var adminMV = new AdminModelView {
                    Email = adminDTO.Email,
                    Perfil = adminDTO.Perfil.ToString()
                };

                adminServico.Incluir(admin);
                
                return Results.Created($"/admin/{admin.Id}", adminMV);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "admin"})
            .WithTags("Admin");

            endpoints.MapGet("/admin/{id}", ([FromRoute]int id, IAdminServico adminServico) => {
                var admin = adminServico.GetById(id);

                var adminMV = new AdminModelView {
                    Email = admin.Email,
                    Perfil = admin.Perfil,
                    Id = admin.Id
                };

                if (admin == null) 
                    return Results.NotFound();
                return Results.Ok(adminMV);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "admin"})
            .WithTags("Admin");

            endpoints.MapGet("/admins", (IAdminServico adminServico) => {
                var admins = adminServico.GetAll();

                var adminsMV = new List<AdminModelView>();

                foreach(Admin item in admins) {
                    adminsMV.Add(new AdminModelView {
                        Id = item.Id,
                        Email = item.Email,
                        Perfil = item.Perfil
                    });
                }

                return Results.Ok(adminsMV);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "admin"})
            .WithTags("Admin");
            #endregion

            #region Veiculos
            ErrosDeValidacao ValidaDTO(VeiculoDTO veiculo) {

                var validacoes = new ErrosDeValidacao {
                    Mensagens = new List<string>()
                };

                if (string.IsNullOrWhiteSpace(veiculo.Nome)) {
                    validacoes.Mensagens.Add("Nome do veículo deve ser informado");
                }
                
                if (string.IsNullOrWhiteSpace(veiculo.Marca)) {
                    validacoes.Mensagens.Add("Marca do veículo deve ser informada");
                }        
                
                if (veiculo.Ano < 1900) {
                    validacoes.Mensagens.Add("Ano do veículo não é válido");
                }
                return validacoes;
            }

            endpoints.MapPost("veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {

                var validacoes = ValidaDTO(veiculoDTO);

                if (validacoes.Mensagens.Count() > 0) 
                    return Results.BadRequest(validacoes);

                var veiculo = new Veiculo {
                    Nome = veiculoDTO.Nome,
                    Ano = veiculoDTO.Ano,
                    Marca = veiculoDTO.Marca
                };
                
                veiculoServico.Incluir(veiculo);
                
                return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "admin, editor "})
            .WithTags("Admin");

            endpoints.MapPut("veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
                var veiculoDB = veiculoServico.GetById(id);

                if (veiculoDB == null)
                    return Results.NotFound();

                var validacoes = ValidaDTO(veiculoDTO);

                if (validacoes.Mensagens.Count() > 0) 
                    return Results.BadRequest(validacoes);

                veiculoDB.Nome = veiculoDTO.Nome;
                veiculoDB.Ano = veiculoDTO.Ano;
                veiculoDB.Marca = veiculoDTO.Marca;
                
                veiculoServico.Alterar(veiculoDB);
                
                return Results.Ok();
            })

            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "admin "})
            .WithTags("Admin");

            endpoints .MapDelete("veiculos/{id}", ([FromRoute]int id, IVeiculoServico veiculoServico) => {
                var veiculoDB = veiculoServico.GetById(id);

                if (veiculoDB == null)
                    return Results.NotFound();

                veiculoServico.Apagar(veiculoDB);
                
                return Results.Ok();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "admin "})
            .WithTags("Admin");

            endpoints.MapGet("/veiculos", ([FromQuery]int pagina, IVeiculoServico veiculoServico) => {
                var veiculos = veiculoServico.GetAll(pagina, "", "");
                return Results.Ok(veiculos);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "admin, editor "})
            .WithTags("Admin");

            endpoints.MapGet("/veiculos/{id}", ([FromRoute]int id, IVeiculoServico veiculoServico) => {
                var veiculo = veiculoServico.GetById(id);
                if (veiculo == null) 
                    return Results.NotFound();
                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "admin, editor "})
            .WithTags("Admin"); 
            #endregion
        });
    }
}