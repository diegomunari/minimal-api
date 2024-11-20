using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infra.Db;

namespace Test.Domain.Servicos;

[TestClass]
public class AdminServicoTest 
{
    private DbContexto CriarDbContextoDeTeste() {
        var assemblypath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblypath ?? "", "..", "..", ".."));

        // configurar o builder
        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory()) // se for nulo, pega o currentdirectory
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }
    
    [TestMethod]
    public void TestandoSalvarAdmin()
    {  
        // Arange
        var contexto = CriarDbContextoDeTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE \"Admins\" ");

        var adm = new Admin();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "admin";

        var adminServico = new AdminServico(contexto);

        // Act
        adminServico.Incluir(adm);

        // Assert
        Assert.AreEqual(1, adminServico.GetAll().Count());

    }

    [TestMethod]
    public void TestandoGetByAdmin()
    {  
        // Arange 
        var contexto = CriarDbContextoDeTeste();

        var adm = new Admin();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "admin";

        var adminServico = new AdminServico(contexto);

        // Act
        adminServico.Incluir(adm);
        var newAdm = adminServico.GetById(adm.Id);

        // Assert
        Assert.AreEqual("teste@teste.com", newAdm.Email);

    }
}  