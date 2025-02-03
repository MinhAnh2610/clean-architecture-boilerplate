using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CleanArchitecture.Application.Validators;

public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
  private readonly UserManager<Domain.Entities.User> _userManager;

  public ResourceOwnerPasswordValidator(
      UserManager<Domain.Entities.User> userManager)
  {
    _userManager = userManager;
  }

  public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
  {
    var user = await _userManager.FindByNameAsync(context.UserName);

    if (user == null)
    {
      context.Result = new GrantValidationResult(
          TokenRequestErrors.InvalidGrant, "Invalid username or password.");
      return;
    }

    var passwordValid = await _userManager.CheckPasswordAsync(user, context.Password);
    if (!passwordValid)
    {
      context.Result = new GrantValidationResult(
          TokenRequestErrors.InvalidGrant, "Invalid username or password.");
      return;
    }

    if (await _userManager.IsLockedOutAsync(user))
    {
      context.Result = new GrantValidationResult(
        TokenRequestErrors.InvalidGrant, "Account is locked.");
      return;
    }

    // Reset failed count on success
    await _userManager.ResetAccessFailedCountAsync(user);

    // User is valid, create claims
    var claims = new List<Claim>
    {
      new Claim(JwtClaimTypes.Subject, user.Id),
      new Claim(JwtClaimTypes.Email, user.Email!),
      new Claim(JwtClaimTypes.Name, user.UserName!),
      new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber!),
      new Claim(JwtClaimTypes.BirthDate, user.BirthDate.ToString()!),
      new Claim(JwtClaimTypes.FamilyName, user.FirstName!),
      new Claim(JwtClaimTypes.GivenName, user.LastName!),
      new Claim(JwtClaimTypes.Gender, user.Gender.ToString())
    };

    var roles = await _userManager.GetRolesAsync(user);
    claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Roles, role)));

    context.Result = new GrantValidationResult(user.Id, "password", claims);
  }
}
