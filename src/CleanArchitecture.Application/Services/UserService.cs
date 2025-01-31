using CleanArchitecture.Application.DTOs.User;
using CleanArchitecture.Application.ServiceContracts;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
    var client = _httpClientFactory.CreateClient();
    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5051/");
    if (disco.IsError)
    {
      return Result<UserProfileResponse>.Failure([UserError.TokenResponseError(disco.Error!)]);
    }

    var accessToken = await _httpContextAccessor.HttpContext!.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
    if (accessToken == null)
      return Result<UserProfileResponse>.Failure([UserError.UnauthorizedUser]);

    var userInfoResponse = await client.GetUserInfoAsync(
      new UserInfoRequest
      {
        Address = disco.UserInfoEndpoint,
        Token = accessToken
      });

    if (userInfoResponse.IsError)
      return Result<UserProfileResponse>.Failure([UserError.UserInfoResponseError(userInfoResponse.Error!)]);

    var id = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)!.Value;
    var userName = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)!.Value;
    var email = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email)!.Value;
    var phoneNumber = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.PhoneNumber)?.Value;
    var birthDate = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.BirthDate)?.Value;
    var firstName = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?.Value;
    var lastname = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName)?.Value;
    var gender = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Gender)!.Value;
    var roles = userInfoResponse.Claims.Where(c => c.Type == JwtClaimTypes.Roles).Select(c => c.Value).ToList();

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
