using Hotexper.Persistence.Data;
using Hotexper.Persistence.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hotexper.Persistence;

public static class RegisterServices
{
    public static IServiceCollection RegisterPersistence(this IServiceCollection services)
    {
        services.ConfigureOptions<DatabaseOptionsSetup>();
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;

            options.UseSqlServer(databaseOptions.ConnectionString, builder =>
            {
                builder.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
                builder.CommandTimeout(databaseOptions.CommandTimeout);
                builder.MigrationsAssembly("Hotexper.Api");
            });

            options.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
            options.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
        });

        return services;
    }
}