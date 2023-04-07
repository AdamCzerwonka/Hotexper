using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Hotexper.Persistence.Options;

public class DatabaseOptionsSetup : IConfigureOptions<DatabaseOptions>
{
    private readonly IConfiguration _configuration;
    private const string DatabaseOptionsSection = "DatabaseOptions";

    public DatabaseOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DatabaseOptions options)
    {
        options.ConnectionString = _configuration.GetConnectionString("dev")!;
        _configuration.GetSection(DatabaseOptionsSection).Bind(options);
    }
}