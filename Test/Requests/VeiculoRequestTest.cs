using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.ModelViews;
using Test.Helpers;

namespace Test.Resquests;

[TestClass]
public class VeiculoRequestTest 
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
        var veiculo = new VeiculoDTO() {
            Marca = "Jeep",
            Nome = "Renegade Longitude",
            Ano = 2024
        };

        var content = new StringContent(JsonSerializer.Serialize(veiculo), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/veiculos", content);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode); 

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<VeiculoDTO>(result, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado?.Marca ?? "");
        Assert.IsNotNull(admLogado?.Nome ?? "");
        Assert.IsNotNull(admLogado?.Ano ?? null);
        
    }
}  