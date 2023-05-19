﻿using System.ComponentModel.DataAnnotations;
using FluentEmail.Core;
using FluentValidation;
using Hotexper.Api.Models;
using Hotexper.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Hotexper.Api.Controllers;

[Route("/api/account")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IFluentEmail _fluentEmail;
    private readonly IValidator<CreateUserModel> _createUserValidator;

    public AccountController(
        ILogger<AccountController> logger,
        UserManager<User> userManager, IFluentEmail fluentEmail, IValidator<CreateUserModel> createUserValidator)
    {
        _logger = logger;
        _userManager = userManager;
        _fluentEmail = fluentEmail;
        _createUserValidator = createUserValidator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateUserModel createUser,
        CancellationToken cancellationToken)
    {
        var validationResult = await _createUserValidator.ValidateAsync(createUser, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = new ErrorModel(StatusCodes.Status422UnprocessableEntity,
                validationResult.Errors.Select(x => x.ErrorMessage));
            return UnprocessableEntity(error);
        }

        var userIdDb = await _userManager.FindByEmailAsync(createUser.Email);
        if (userIdDb is not null)
        {
            var error = new ErrorModel(StatusCodes.Status422UnprocessableEntity,
                new List<string>() { "User with given email already exists" });
            return UnprocessableEntity(error);
        }

        var user = new User()
        {
            UserName = createUser.Firstname + createUser.Lastname,
            Email = createUser.Email,
            PhoneNumber = createUser.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, createUser.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"http://localhost:3000/register/verify?userId={user.Id}&token={token}";

        await _fluentEmail.To(user.Email).Body(url).SendAsync(cancellationToken);
        return Ok();
    }

    [HttpPost("{id:guid}/verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailModel verifyEmailModel, Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return BadRequest();
        }

        var result = await _userManager.ConfirmEmailAsync(user, verifyEmailModel.Token);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest();
    }
}

public record VerifyEmailModel(string Token);