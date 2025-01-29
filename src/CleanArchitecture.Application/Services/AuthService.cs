using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Application.ServiceContracts;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Services;

public class AuthService : IAuthService
{
  private readonly UserManager<User> _userManager;
  private readonly IHttpClientFactory _httpClientFactory;

  public AuthService(UserManager<User> userManager, IHttpClientFactory httpClientFactory)
  {
    _userManager = userManager;
    _httpClientFactory = httpClientFactory;
  }

  public Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
  {
    throw new NotImplementedException();
  }

  public async Task<Result<string>> LoginAsync(LoginRequest loginRequest)
  {
    var user = await _userManager.FindByEmailAsync(loginRequest.Email);
    if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
    {
      return Result<string>.Failure(AuthErrors.InvalidCredentials);
    }

    var client = _httpClientFactory.CreateClient();
    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5051/");
    if (disco.IsError)
    {
      return Result<string>.Failure(AuthErrors.IdentityServerFailed);
    }

    var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
      Address = "https://localhost:5051/connect/token",

      ClientId = "api_client",
      ClientSecret = "secret",

      Scope = "API"
    });

    if (tokenResponse.IsError)
    {
      return Result<string>.Failure(AuthErrors.TokenResponseError(tokenResponse.Error!));
    }

    return Result<string>.Success(tokenResponse.AccessToken!);
  }

  public Task<Result<string>> RegisterAsync(RegisterRequest registerRequest)
  {
    throw new NotImplementedException();
  }

  public Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
  {
    throw new NotImplementedException();
  }
}
