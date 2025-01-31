using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.Validators.Auth;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequest>
{
  public ForgotPasswordValidator()
  {
    RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
  }
}
