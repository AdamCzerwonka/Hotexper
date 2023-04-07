﻿using Hotexper.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hotexper.Api.Controllers;

[Route("/api/account")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<User> _userManager;

    public AccountController(ILogger<AccountController> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateUserModel createUser, CancellationToken cancellationToken)
    {
        var userIdDb = await _userManager.FindByEmailAsync(createUser.Email);
        if (userIdDb is not null)
        {
            return BadRequest(new { Error = "User with given email already exists" });
        }
        
        var user = new User()
        {
            UserName = createUser.Firstname + createUser.Lastname,
            Email = createUser.Email,
            PhoneNumber = createUser.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, createUser.Password);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }
}

public record CreateUserModel(string Email, string Firstname, string Lastname, string PhoneNumber, string Password,
    string Password2);