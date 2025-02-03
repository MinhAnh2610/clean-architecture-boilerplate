using CleanArchitecture.Application.DTOs.User;

namespace CleanArchitecture.Application.Validators.User;

public class UserValidator : AbstractValidator<UserRequest>
{
  public UserValidator()
  {
    RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.");
  }
}
