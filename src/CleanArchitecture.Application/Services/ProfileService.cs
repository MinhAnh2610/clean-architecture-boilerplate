using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CleanArchitecture.Application.Services;

public class ProfileService : IProfileService
{
  private readonly UserManager<User> _userManager;

  public ProfileService(UserManager<User> userManager)
  {
    _userManager = userManager;
  }

  public async Task GetProfileDataAsync(ProfileDataRequestContext context)
  {
    var subjectId = context.Subject.GetSubjectId(); // Get user ID from the token
    var user = await _userManager.FindByIdAsync(subjectId);

    if (user != null)
    {
      var claims = new List<Claim>
      {
        // Add standard claims
        new Claim(JwtClaimTypes.Subject, user.Id),
        new Claim(JwtClaimTypes.Name, user.UserName!),
        new Claim(JwtClaimTypes.Email, user.Email!),
          
        // Add custom claims with user profile data
        new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber!),
        new Claim(JwtClaimTypes.BirthDate, user.BirthDate.ToString()!),
        new Claim(JwtClaimTypes.FamilyName, user.FirstName!),
        new Claim(JwtClaimTypes.GivenName, user.LastName!),
        new Claim(JwtClaimTypes.Gender, user.Gender.ToString())
        //new Claim("UserProfile", $"{user.FirstName} {user.LastName}"),        
        //new Claim("ProfilePicture", user.ProfilePictureUrl ?? string.Empty)
      };

      // Add any roles the user belongs to
      var roles = await _userManager.GetRolesAsync(user);
      claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Roles, role)));

      // Include the claims in the token
      context.IssuedClaims.AddRange(claims);
    }
  }

  public Task IsActiveAsync(IsActiveContext context)
  {
    return Task.CompletedTask;
  }
}
