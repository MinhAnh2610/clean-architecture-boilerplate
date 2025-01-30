using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CleanArchitecture.Application.Validators;

public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
  private readonly UserManager<User> _userManager;

  public ResourceOwnerPasswordValidator(
      UserManager<User> userManager)
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

    // User is valid, create claims
    var claims = new List<Claim>
        {
            new Claim("sub", user.Id),
            new Claim("email", user.Email!),
            new Claim("name", user.UserName!)
        };

    context.Result = new GrantValidationResult(user.Id, "password", claims);
  }
}
