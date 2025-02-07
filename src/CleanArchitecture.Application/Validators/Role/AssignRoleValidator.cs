using CleanArchitecture.Application.DTOs.Role;

namespace CleanArchitecture.Application.Validators.Role;

public class AssignRoleValidator : AbstractValidator<AssignRoleRequest>
{
  public AssignRoleValidator()
  {
    RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.");

    RuleFor(x => x.Roles)
        .NotEmpty().WithMessage("Roles cannot be empty.")
        .Must(roles => roles != null && roles.All(r => !string.IsNullOrWhiteSpace(r)))
        .WithMessage("Each role must be a non-empty string.");
  }
}
