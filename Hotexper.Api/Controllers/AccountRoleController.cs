using Hotexper.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hotexper.Api.Controllers;

[ApiController]
[Route("/api/account/{userId:guid}/role")]
public class AccountRoleController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AccountRoleController> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountRoleController(UserManager<User> userManager, ILogger<AccountRoleController> logger, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _logger = logger;
        _roleManager = roleManager;
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

    [HttpPut]
    public async Task<IActionResult> UpdateUserRoles(Guid userId, [FromBody] UpdateUserRolesModel model)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var rolesToAdd = model.Roles.Where(x=>!userRoles.Contains(x));
        var rolesToRemove = userRoles.Where(x => !model.Roles.Contains(x));
        _logger.LogInformation("Roles to add: {roles}", rolesToAdd);
        _logger.LogInformation("Roles to remove: {roles}", rolesToRemove);

        //TODO: in case of failure this part may fail only partially
        var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        if (!addResult.Succeeded && !removeResult.Succeeded)
        {
            return BadRequest();
        }

        return Ok();
    }
}

public record AddToRoleModel(string RoleName);
public record UpdateUserRolesModel(List<string> Roles);