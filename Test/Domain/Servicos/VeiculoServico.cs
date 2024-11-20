using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infra.Db;

namespace Test.Domain.Servicos;

[TestClass]
public class VeiculoServicoTest 
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
    public void TestandoSalvarVeiculo()
    {  
        // Arange
        var contexto = CriarDbContextoDeTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE \"Veiculos\" ");

        var veiculo = new Veiculo();
        veiculo.Ano = 2024;
        veiculo.Marca = "Jeep";
        veiculo.Nome = "Renegade Longitude";

        var veiculoServico = new VeiculoServico(contexto);

        // Act
        veiculoServico.Incluir(veiculo);

        // Assert
        Assert.AreEqual(1, veiculoServico.GetAll().Count());

    }

    [TestMethod]
    public void TestandoGetByAdmin()
    {  
        // Arange 
        var contexto = CriarDbContextoDeTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE \"Veiculos\" ");

        var veiculo = new Veiculo();
        veiculo.Ano = 2024;
        veiculo.Marca = "Jeep";
        veiculo.Nome = "Renegade Longitude";

        var veiculoServico = new VeiculoServico(contexto);

        // Act
        veiculoServico.Incluir(veiculo);
        var newAdm = veiculoServico.GetById(veiculo.Id);

        // Assert
        Assert.AreEqual("Jeep", newAdm.Marca);
    }
}  