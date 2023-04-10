using Hotexper.Api.Services;
using Hotexper.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hotexper.Api.Controllers;

[Route("/api/login")]
public class LoginController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _tokenService;

    public LoginController(UserManager<User> userManager,
        IJwtTokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var user = await _userManager.FindByEmailAsync(loginModel.Email);
        if (user is null)
        {
            return BadRequest();
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginModel.Password);
        if (!isPasswordCorrect)
        {
            return BadRequest();
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(token);
    }
}

public record LoginModel(string Email, string Password);