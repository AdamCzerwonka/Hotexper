using FluentEmail.Core;
using FluentValidation;
using Hotexper.Api.Models;
using Hotexper.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
        }

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
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _fluentEmail.To(user.Email).Body(token).SendAsync(cancellationToken);
        return Ok();
    }

    [HttpPost("email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailModel verifyEmailModel)
    {
        var user = await _userManager.FindByIdAsync(verifyEmailModel.UserId);
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

public record VerifyEmailModel(string UserId, string Token);