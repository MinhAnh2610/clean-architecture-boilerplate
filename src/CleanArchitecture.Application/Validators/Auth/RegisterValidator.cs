using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
  public RegisterValidator()
  {
    RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

    RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Invalid phone number format.");

    RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.");

    RuleFor(x => x.PasswordConfirmation)
        .Equal(x => x.Password).WithMessage("Passwords do not match.");

    RuleFor(x => x.UserName)
        .NotEmpty().WithMessage("Username is required.");

    RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Matches(@"^[a-zA-Z]+$").WithMessage("First name must contain only letters.");

    RuleFor(x => x.LastName)
        .NotEmpty().WithMessage("Last name is required.")
        .Matches(@"^[a-zA-Z]+$").WithMessage("Last name must contain only letters.");
  }
}
