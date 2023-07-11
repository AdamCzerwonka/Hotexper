using Microsoft.AspNetCore.Identity;

namespace Hotexper.Api.DTOs;

public class RoleResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;

    public static RoleResponse MapToResponse(IdentityRole role)
        => new()
        {
            Id = role.Id,
            Name = role.Name!
        };
}