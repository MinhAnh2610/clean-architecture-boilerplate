using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.Validators.Auth;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
{
  public ResetPasswordValidator()
  {
    RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

    RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.");

    RuleFor(x => x.PasswordConfirmation)
        .Equal(x => x.Password).WithMessage("Passwords do not match.");

    RuleFor(x => x.AccessToken)
        .NotEmpty().WithMessage("Access token is required.");
  }
}
