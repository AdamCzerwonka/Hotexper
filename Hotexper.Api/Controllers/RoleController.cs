using System.Net;
using Hotexper.Api.DTOs;
using Hotexper.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotexper.Api.Controllers;

[ApiController]
[Route("/api/role")]
public class RoleController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        var role = new IdentityRole()
        {
            Name = name
        };
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var response = new ErrorModel(HttpStatusCode.BadRequest, result.Errors.Select(x => x.Description));
            return BadRequest(response);
        }

        return Created("test", RoleResponse.MapToResponse(role));
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<string>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var roles = await _roleManager
            .Roles
            .AsNoTracking()
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        return Ok(roles);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
        {
            return NotFound();
        }

        return Ok(RoleResponse.MapToResponse(role));
    }
}