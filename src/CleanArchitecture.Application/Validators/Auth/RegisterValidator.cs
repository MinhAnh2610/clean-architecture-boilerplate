using CleanArchitecture.Application.DTOs.Auth;
using FluentValidation;

namespace CleanArchitecture.Application.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
  public RegisterValidator()
  {
    RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

    RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.");

    RuleFor(x => x.PasswordConfirmation)
        .Equal(x => x.Password).WithMessage("Passwords do not match.");

    RuleFor(x => x.UserName)
        .NotEmpty().WithMessage("Username is required.");
  }
}
