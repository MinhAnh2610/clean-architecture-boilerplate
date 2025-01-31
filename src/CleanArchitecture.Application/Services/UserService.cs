using CleanArchitecture.Application.DTOs.User;
using CleanArchitecture.Application.ServiceContracts;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;

namespace CleanArchitecture.Application.Services;

public class UserService : IUserService
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IHttpClientFactory _httpClientFactory;

  public UserService(IHttpContextAccessor httpContextAccessor, 
                     IHttpClientFactory httpClientFactory)
  {
    _httpContextAccessor = httpContextAccessor;
    _httpClientFactory = httpClientFactory;
  }

  public async Task<Result<UserProfileResponse>> GetUserProfile()
  {
    var user = _httpContextAccessor.HttpContext?.User;
    if (user == null || !user.Identity!.IsAuthenticated)
    {
      return Result<UserProfileResponse>.Failure([UserError.UnauthorizedUser]);
    }

    var id = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    var userName = user.FindFirst(JwtClaimTypes.Name)!.Value;
    var email = user.FindFirst(ClaimTypes.Email)!.Value;
    var phoneNumber = user.FindFirst(JwtClaimTypes.PhoneNumber)?.Value;
    var birthDate = user.FindFirst(ClaimTypes.DateOfBirth)?.Value;
    var firstName = user.FindFirst(ClaimTypes.Surname)?.Value;
    var lastname = user.FindFirst(ClaimTypes.GivenName)?.Value;
    var gender = user.FindFirst(ClaimTypes.Gender)!.Value;
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

    return Result<UserProfileResponse>.Success(new UserProfileResponse
    {
      Id = id,
      UserName = userName,
      Email = email,
      PhoneNumber = phoneNumber,
      BirthDate = (birthDate == null) ? default : DateOnly.Parse(birthDate!),
      FirstName = firstName,
      LastName = lastname,
      Gender = Boolean.Parse(gender),
      Roles = roles
    });
  }

  public Task<Result<UserProfileResponse>> UpdateUserProfileAsync(UpdateProfileRequest updateProfileRequest)
  {
    throw new NotImplementedException();
  }
}
