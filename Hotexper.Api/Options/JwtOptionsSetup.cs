using Microsoft.Extensions.Options;

namespace Hotexper.Api.Options;

public class JwtOptionsSetup:IConfigureOptions<JwtOptions>
{
    private readonly IConfiguration _configuration;
    private const string JwtOptionsSection = "JwtOptions";

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Configure(JwtOptions options)
    {
       _configuration.GetSection(JwtOptionsSection).Bind(options); 
    }
}