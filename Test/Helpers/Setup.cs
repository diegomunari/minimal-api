using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using Test.Mocks;

namespace Test.Helpers;

public class Setup 
{
    public const string PORT = "5001";
    public static TestContext testContext = default!;
    public static WebApplicationFactory<StartUp> http = default!;
    public static HttpClient client = default!;

    public static void ClassInit(TestContext testContext)
    {
        Setup.testContext = testContext;
        Setup.http = new WebApplicationFactory<StartUp>();

        Setup.http = Setup.http.WithWebHostBuilder(builder => {
            builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");

            builder.ConfigureServices(services => {
                services.AddScoped<IAdminServico, AdminServiceMock>();
                services.AddScoped<IVeiculoServico, VeiculoServiceMock>();
            });
        });

        Setup.client = Setup.http.CreateClient();
    }

    public static void ClassCleanUp()
    {
        Setup.http.Dispose();
    }
}