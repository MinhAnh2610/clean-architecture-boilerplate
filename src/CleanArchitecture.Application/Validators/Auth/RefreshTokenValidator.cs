using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.Validators.Auth;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
  public RefreshTokenValidator()
  {
    RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh Token is required.");
  }
}
