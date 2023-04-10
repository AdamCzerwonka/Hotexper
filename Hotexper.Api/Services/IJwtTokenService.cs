using Hotexper.Domain.Entities;

namespace Hotexper.Api.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}