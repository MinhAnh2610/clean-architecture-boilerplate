using CleanArchitecture.Application.DTOs.User;

namespace CleanArchitecture.Application.Validators.User;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
{
  public UpdateProfileValidator()
  {
    RuleFor(x => x.PhoneNumber)
        .Matches(@"^\+?[1-9]\d{1,14}$") // E.164 phone number format
        .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
        .WithMessage("Invalid phone number format.");

    RuleFor(x => x.BirthDate)
        .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("BirthDate must be in the past.")
        .When(x => x.BirthDate.HasValue);
  }
}
