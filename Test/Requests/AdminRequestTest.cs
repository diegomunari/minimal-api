using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.ModelViews;
using Test.Helpers;

namespace Test.Resquests;

[TestClass]
public class AdminRequestTest 
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext) {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanUp()
    {
        Setup.ClassCleanUp();
    }

    [TestMethod]
     public async Task TestarGetSetPropriedades()
    {
        // Arange
        var loginDTO = new LoginDTO() {
            Email = "admin@teste.com",
            Senha = "1234"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/admin/login", content);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode); 

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdminLogado>(result, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado?.Email ?? "");
        Assert.IsNotNull(admLogado?.Token ?? "");
        Assert.IsNotNull(admLogado?.Perfil ?? "");
        
    }
}  