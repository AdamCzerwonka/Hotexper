using Hotexper.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Hotexper.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleRepository(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task CreateAsync(List<IdentityRole> roles)
    {
        foreach (var role in roles)
        {
            await _roleManager.CreateAsync(role);
        }
    }
}