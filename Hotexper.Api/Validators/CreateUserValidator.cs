using FluentValidation;
using Hotexper.Api.Models;

namespace Hotexper.Api.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserModel>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Incorrect email format");

        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("Firstname cannot be empty")
            .MaximumLength(50).WithMessage("Firstname cannot be longer than 50 characters");

        RuleFor(x => x.Lastname)
            .NotEmpty().WithMessage("Lastname cannot be empty")
            .MaximumLength(50).WithMessage("Lastname cannot be longer than 50 chacaters");

        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

        RuleFor(x => x.Password2)
            .Equal(x => x.Password).WithMessage("Password2 must match password");

        RuleFor(x => x.PhoneNumber)
            .Matches("\\d{9}").WithMessage("Phone number must contain 9 numbers");
    }
}