using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class AdminTest 
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arange
        var adm = new Admin();

        // Act
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "admin";

        // Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Senha );
        Assert.AreEqual("admin", adm.Perfil);
    }
}  