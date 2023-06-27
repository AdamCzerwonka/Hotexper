using Hotexper.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hotexper.Api.Controllers;

[ApiController]
[Route("/api/account/{userId:guid}/role")]
public class AccountRoleController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AccountRoleController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }


    [HttpPost]
    public async Task<IActionResult> AddToRole(Guid userId, [FromBody] AddToRoleModel model)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var result = await _userManager.AddToRoleAsync(user, model.RoleName);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> RemoveFromRole(Guid userId, [FromBody] AddToRoleModel model)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        return Ok();
    }
}

public record AddToRoleModel(string RoleName);