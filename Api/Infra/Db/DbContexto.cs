using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Infra.Db;
public class DbContexto : DbContext {   

    private readonly IConfiguration _configAppSettings;

    public DbContexto(IConfiguration configuration){
        _configAppSettings = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
            var conn = _configAppSettings.GetConnectionString("ConexaoPadrao").ToString();
            if (!string.IsNullOrEmpty(conn))
                optionsBuilder.UseNpgsql(conn);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Admin>().HasData(
            new Admin {
                Id = 1,
                Email = "admin@teste.com",
                Senha = "1234",
                Perfil = "admin"
            }
        );
    }

    public DbSet<Admin> Admins { get;  set; }
    public DbSet<Veiculo> Veiculos { get; set; }

}
