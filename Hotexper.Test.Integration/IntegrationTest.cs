using Hotexper.Persistence.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
                    services.RemoveAll(typeof(AppDbContext));
                    services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("testDb"); });
                });
            });
        HttpClient = appFactory.CreateClient();
    }
}