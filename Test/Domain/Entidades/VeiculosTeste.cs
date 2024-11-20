using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class VeiculoTest 
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arange
        var veiculo = new Veiculo();

        // Act
        veiculo.Marca = "Teste";
        veiculo.Nome = "Teste";
        veiculo.Ano = 1980;
        veiculo.Id = 1;

        // Assert
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("Teste", veiculo.Marca);
        Assert.AreEqual("Teste", veiculo.Nome);
        Assert.AreEqual(1980, veiculo.Ano);
    }
}  