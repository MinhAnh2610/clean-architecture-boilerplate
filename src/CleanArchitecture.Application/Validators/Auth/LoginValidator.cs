using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginRequest>
{
  public LoginValidator()
  {
    RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.");

    RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.");
  }
}
