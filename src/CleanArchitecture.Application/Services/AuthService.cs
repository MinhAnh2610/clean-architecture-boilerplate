using Castle.Core.Resource;
using CleanArchitecture.Application.DTOs.Auth;
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

  public async Task<Result<AuthResponse>> LoginAsync(LoginRequest loginRequest)
  {
    var user = await _userManager.FindByNameAsync(loginRequest.UserName);
    if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
    {
      return Result<AuthResponse>.Failure([AuthErrors.InvalidCredentials]);
    }

    var client = _httpClientFactory.CreateClient();
    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5051/");
    if (disco.IsError)
    {
      return Result<AuthResponse>.Failure([AuthErrors.IdentityServerFailed]);
    }

    var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
    {
      Address = disco.TokenEndpoint,

      ClientId = "api_client",
      ClientSecret = "secret",

      UserName = "Admin123",
      Password = "12345",

      Scope = "openid profile email roles API offline_access"
    });
    
    if (tokenResponse.IsError)
    {
      return Result<AuthResponse>.Failure([AuthErrors.TokenResponseError(tokenResponse.Error!)]);
    }

    return Result<AuthResponse>.Success(new AuthResponse
    {
      AccessToken = tokenResponse.AccessToken!,
      AccessTokenExpiration = tokenResponse.ExpiresIn,
      RefreshToken = tokenResponse.RefreshToken!,
      RefreshTokenExpiration = 2592000,
      Email = user.Email!,
      UserName = user.UserName!,
    });
  }

  public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest registerRequest)
  {
    if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
    {
      return Result<AuthResponse>.Failure([AuthErrors.AlreadyRegistered]);
    }
    if (await _userManager.FindByNameAsync(registerRequest.UserName) != null)
    {
      return Result<AuthResponse>.Failure([AuthErrors.DuplicateUserName]);
    }
    if (!registerRequest.Password.Equals(registerRequest.PasswordConfirmation))
    {
      return Result<AuthResponse>.Failure([AuthErrors.NotEqualPassword]);
    }

    var user = new User { UserName = registerRequest.UserName, Email = registerRequest.Email };

    var result = await _userManager.CreateAsync(user, registerRequest.Password);
    if (!result.Succeeded)
    {
      List<Error> Errors = new List<Error>();
      foreach (var error in result.Errors)
      {
        Errors.Add(new Error(error.Code, error.Description));
      }
      return Result<AuthResponse>.Failure(Errors);
    }
    await _userManager.AddToRolesAsync(user, ["Customer"]);

    var client = _httpClientFactory.CreateClient();
    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5051/");
    if (disco.IsError)
    {
      return Result<AuthResponse>.Failure([AuthErrors.IdentityServerFailed]);
    }

    var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
    {
      Address = disco.TokenEndpoint,

      ClientId = "api_client",
      ClientSecret = "secret",

      UserName = "Admin123",
      Password = "12345",

      Scope = "openid profile email roles API offline_access"
    });

    if (tokenResponse.IsError)
    {
      return Result<AuthResponse>.Failure([AuthErrors.TokenResponseError(tokenResponse.Error!)]);
    }

    return Result<AuthResponse>.Success(new AuthResponse
    {
      AccessToken = tokenResponse.AccessToken!,
      AccessTokenExpiration = tokenResponse.ExpiresIn,
      RefreshToken = tokenResponse.RefreshToken!,
      RefreshTokenExpiration = 2592000,
      Email = registerRequest.Email!,
      UserName = registerRequest.UserName!,
    });
  }

  public Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
  {
    throw new NotImplementedException();
  }
}
