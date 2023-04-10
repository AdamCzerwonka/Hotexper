using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hotexper.Api.Options;
using Hotexper.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hotexper.Api.Services;

public class JwtTokenService : IJwtTokenService
{
   private readonly JwtOptions _options;

   public JwtTokenService(IOptions<JwtOptions> options)
   {
      _options = options.Value;
   }
   
   public string GenerateToken(User user)
   {
       var key = Encoding.UTF8.GetBytes(_options.SecretKey);
       var tokenDescriptor = new SecurityTokenDescriptor
       {
           Subject = new ClaimsIdentity(new[]
           {
               new Claim("Id", Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
               new Claim(JwtRegisteredClaimNames.Email, user.Email!),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
           }),
           Expires = DateTime.UtcNow.AddHours(1),
           Issuer = _options.Issuer,
           Audience = _options.Audience,
           SigningCredentials =
               new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
       };

       var tokenHandler = new JwtSecurityTokenHandler();
       var token = tokenHandler.CreateToken(tokenDescriptor);
       var stringToken = tokenHandler.WriteToken(token);

       return stringToken;
   }
}