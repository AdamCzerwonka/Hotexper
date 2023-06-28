using Hotexper.Persistence.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hotexper.Test.Integration;

public class IntegrationTest
{
    protected readonly HttpClient HttpClient;

    protected IntegrationTest()
    {
        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor =
                        services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    services.Remove(descriptor!);
                    services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("testDb"); });
                });
            });
        HttpClient = appFactory.CreateClient();
    }
}