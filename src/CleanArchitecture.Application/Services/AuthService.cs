using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.Enums;
using CleanArchitecture.Application.ServiceContracts;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Services;

public class AuthService : IAuthService
{
  private readonly UserManager<User> _userManager;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly IValidator<LoginRequest> _loginValidator;
  private readonly IValidator<RegisterRequest> _registerValidator;
  private readonly IValidator<ForgotPasswordRequest> _forgotPasswordValidator;
  private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;

  public AuthService(UserManager<User> userManager,
                     IHttpClientFactory httpClientFactory,
                     IValidator<LoginRequest> loginValidator,
                     IValidator<RegisterRequest> registerValidator,
                     IValidator<ForgotPasswordRequest> forgotPasswordValidator,
                     IValidator<ResetPasswordRequest> resetPasswordValidator)
  {
    _userManager = userManager;
    _httpClientFactory = httpClientFactory;
    _loginValidator = loginValidator;
    _registerValidator = registerValidator;
    _forgotPasswordValidator = forgotPasswordValidator;
    _resetPasswordValidator = resetPasswordValidator;
  }

  public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
  {
    var validationResult = await _forgotPasswordValidator.ValidateAsync(forgotPasswordRequest);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<string>.Failure(errors, StatusCodes.Status400BadRequest);
    }
    var user = await _userManager.FindByEmailAsync(forgotPasswordRequest.Email);
    if (user == null)
    {
      return Result<string>.Failure([AuthErrors.UserNotFound], StatusCodes.Status404NotFound);
    }

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

    // later you need to configure the email for sending the reset token

    return Result<string>.Success(token, StatusCodes.Status200OK);
  }

  public async Task<Result<AuthResponse>> LoginAsync(LoginRequest loginRequest)
  {
    var validationResult = await _loginValidator.ValidateAsync(loginRequest);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<AuthResponse>.Failure(errors, StatusCodes.Status400BadRequest);
    }

    var user = await _userManager.FindByNameAsync(loginRequest.UserName);
    if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
    {
      return Result<AuthResponse>.Failure([AuthErrors.InvalidCredentials], StatusCodes.Status400BadRequest);
    }

    var client = _httpClientFactory.CreateClient();
    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5051/");
    if (disco.IsError)
    {
      return Result<AuthResponse>.Failure([AuthErrors.IdentityServerFailed], (int)StatusCodes.Status500InternalServerError);
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
      return Result<AuthResponse>.Failure([AuthErrors.TokenResponseError(tokenResponse.Error!)], (int)StatusCodes.Status500InternalServerError);
    }

    return Result<AuthResponse>.Success(new AuthResponse
    {
      AccessToken = tokenResponse.AccessToken!,
      AccessTokenExpiration = tokenResponse.ExpiresIn,
      RefreshToken = tokenResponse.RefreshToken!,
      RefreshTokenExpiration = 2592000,
      Email = user.Email!,
      UserName = user.UserName!,
    }, StatusCodes.Status200OK);
  }

  public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest registerRequest)
  {
    if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
    {
      return Result<AuthResponse>.Failure([AuthErrors.AlreadyRegistered], StatusCodes.Status409Conflict);
    }
    if (await _userManager.FindByNameAsync(registerRequest.UserName) != null)
    {
      return Result<AuthResponse>.Failure([AuthErrors.DuplicateUserName], StatusCodes.Status409Conflict);
    }
    var validationResult = await _registerValidator.ValidateAsync(registerRequest);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<AuthResponse>.Failure(errors, StatusCodes.Status400BadRequest);
    }

    var user = new User { UserName = registerRequest.UserName, Email = registerRequest.Email };

    var result = await _userManager.CreateAsync(user, registerRequest.Password);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
      return Result<AuthResponse>.Failure(errors, StatusCodes.Status500InternalServerError);
    }
    await _userManager.AddToRolesAsync(user, [Roles.Customer]);

    var client = _httpClientFactory.CreateClient();
    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5051/");
    if (disco.IsError)
    {
      return Result<AuthResponse>.Failure([AuthErrors.IdentityServerFailed], StatusCodes.Status500InternalServerError);
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
      return Result<AuthResponse>.Failure([AuthErrors.TokenResponseError(tokenResponse.Error!)], StatusCodes.Status500InternalServerError);
    }

    return Result<AuthResponse>.Success(new AuthResponse
    {
      AccessToken = tokenResponse.AccessToken!,
      AccessTokenExpiration = tokenResponse.ExpiresIn,
      RefreshToken = tokenResponse.RefreshToken!,
      RefreshTokenExpiration = 2592000,
      Email = registerRequest.Email!,
      UserName = registerRequest.UserName!,
    }, StatusCodes.Status200OK);
  }

  public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
  {
    var validationResult = await _resetPasswordValidator.ValidateAsync(resetPasswordRequest);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<string>.Failure(errors, StatusCodes.Status400BadRequest);
    }
    var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
    if (user == null)
    {
      return Result<string>.Failure([AuthErrors.UserNotFound], StatusCodes.Status404NotFound);
    }

    var result = await _userManager.ResetPasswordAsync(user, resetPasswordRequest.AccessToken, resetPasswordRequest.Password);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
      return Result<string>.Failure(errors, StatusCodes.Status500InternalServerError);
    }

    return Result<string>.Success(null, StatusCodes.Status200OK);
  }
}
