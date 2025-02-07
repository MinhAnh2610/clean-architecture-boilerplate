using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.DTOs.User;
using CleanArchitecture.Application.Enums;
using CleanArchitecture.Application.ServiceContracts;
using CleanArchitecture.Application.Validators.User;
using CleanArchitecture.Domain.Entities;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CleanArchitecture.Application.Services;

public class AuthService : IAuthService
{
  private readonly UserManager<User> _userManager;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IValidator<LoginRequest> _loginValidator;
  private readonly IValidator<RegisterRequest> _registerValidator;
  private readonly IValidator<ForgotPasswordRequest> _forgotPasswordValidator;
  private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
  private readonly IValidator<DTOs.Auth.RefreshTokenRequest> _refreshTokenValidator;

  public AuthService(UserManager<User> userManager,
                     IHttpClientFactory httpClientFactory,
                     IValidator<LoginRequest> loginValidator,
                     IValidator<RegisterRequest> registerValidator,
                     IValidator<ForgotPasswordRequest> forgotPasswordValidator,
                     IValidator<ResetPasswordRequest> resetPasswordValidator,
                     IValidator<DTOs.Auth.RefreshTokenRequest> refreshTokenValidator,
                     IHttpContextAccessor httpContextAccessor
                     )
  {
    _userManager = userManager;
    _httpClientFactory = httpClientFactory;
    _loginValidator = loginValidator;
    _registerValidator = registerValidator;
    _forgotPasswordValidator = forgotPasswordValidator;
    _resetPasswordValidator = resetPasswordValidator;
    _refreshTokenValidator = refreshTokenValidator;
    _httpContextAccessor = httpContextAccessor; 
  }

  public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
  {
    var validationResult = await _forgotPasswordValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<string>.Failure(errors, StatusCodes.Status400BadRequest);
    }
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user == null)
    {
      return Result<string>.Failure([AuthErrors.UserNotFound], StatusCodes.Status404NotFound);
    }

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

    // later you need to configure the email for sending the reset token

    return Result<string>.Success(token, StatusCodes.Status200OK);
  }

  public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
  {
    var validationResult = await _loginValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<AuthResponse>.Failure(errors, StatusCodes.Status400BadRequest);
    }

    var user = await _userManager.FindByNameAsync(request.UserName);
    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
    {
      return Result<AuthResponse>.Failure([AuthErrors.InvalidCredentials], StatusCodes.Status400BadRequest);
    }

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

      UserName = user.UserName!,
      Password = request.Password,

      Scope = "openid profile email roles API offline_access"
    });

    if (tokenResponse.IsError)
    {
      return Result<AuthResponse>.Failure([AuthErrors.TokenResponseError(tokenResponse.ErrorDescription!)], StatusCodes.Status400BadRequest);
    }

    user.RefreshToken = tokenResponse.RefreshToken;
    user.RefreshTokenExpiration = DateTime.UtcNow + TimeSpan.FromDays(30);

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

  public async Task<Result<AuthResponse>> RefreshTokenAsync(DTOs.Auth.RefreshTokenRequest request)
  {
    var validationResult = await _refreshTokenValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<AuthResponse>.Failure(errors, StatusCodes.Status400BadRequest);
    }

    var authorizedUser = _httpContextAccessor.HttpContext?.User;
    if (authorizedUser == null || !authorizedUser.Identity!.IsAuthenticated)
    {
      return Result<AuthResponse>.Failure([UserErrors.UnauthorizedUser], StatusCodes.Status401Unauthorized);
    }
    var id = authorizedUser.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    var user = await _userManager.FindByIdAsync(id);

    var client = _httpClientFactory.CreateClient();
    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5051/");
    if (disco.IsError)
    {
      return Result<AuthResponse>.Failure([AuthErrors.IdentityServerFailed], StatusCodes.Status500InternalServerError);
    }

    var tokenResponse = await client.RequestRefreshTokenAsync(new IdentityModel.Client.RefreshTokenRequest
    {
      Address = disco.TokenEndpoint,
      GrantType = "refresh_token",

      ClientId = "api_client",
      ClientSecret = "secret",

      RefreshToken = request.RefreshToken,

      //Scope = "openid profile email roles API offline_access"
    });

    if (tokenResponse.IsError)
      return Result<AuthResponse>.Failure([AuthErrors.TokenResponseError(tokenResponse.ErrorDescription!)], StatusCodes.Status400BadRequest);

    user!.RefreshToken = tokenResponse.RefreshToken;
    user.RefreshTokenExpiration = DateTime.UtcNow + TimeSpan.FromDays(30);

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

  public async Task<Result<string>> RegisterAsync(RegisterRequest request)
  {
    if (await _userManager.FindByEmailAsync(request.Email) != null)
    {
      return Result<string>.Failure([AuthErrors.AlreadyRegistered], StatusCodes.Status409Conflict);
    }
    if (await _userManager.FindByNameAsync(request.UserName) != null)
    {
      return Result<string>.Failure([AuthErrors.DuplicateUserName], StatusCodes.Status409Conflict);
    }
    var validationResult = await _registerValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<string>.Failure(errors, StatusCodes.Status400BadRequest);
    }

    var user = new User 
    { 
      UserName = request.UserName, 
      Email = request.Email,
      Gender = request.Gender,
      FirstName = request.FirstName,
      LastName = request.LastName,
      PhoneNumber = request.PhoneNumber
    };

    var result = await _userManager.CreateAsync(user, request.Password);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
      return Result<string>.Failure(errors, StatusCodes.Status500InternalServerError);
    }
    var roleResult = await _userManager.AddToRolesAsync(user, [Roles.Customer]);
    if (!roleResult.Succeeded)
    {
      var errors = roleResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
      return Result<string>.Failure(errors, StatusCodes.Status500InternalServerError);
    }

    //var client = _httpClientFactory.CreateClient();
    //var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5051/");
    //if (disco.IsError)
    //{
    //  return Result<string>.Failure([AuthErrors.IdentityServerFailed], StatusCodes.Status500InternalServerError);
    //}

    //var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
    //{
    //  Address = disco.TokenEndpoint,

    //  ClientId = "api_client",
    //  ClientSecret = "secret",

    //  UserName = user.UserName,
    //  Password = registerRequest.Password,

    //  Scope = "openid profile email roles API offline_access"
    //});

    //if (tokenResponse.IsError)
    //{
    //  return Result<string>.Failure([AuthErrors.TokenResponseError(tokenResponse.Error!)], StatusCodes.Status500InternalServerError);
    //}

    return Result<string>.Success(null, StatusCodes.Status200OK);
  }

  public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request)
  {
    var validationResult = await _resetPasswordValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<string>.Failure(errors, StatusCodes.Status400BadRequest);
    }
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user == null)
    {
      return Result<string>.Failure([AuthErrors.UserNotFound], StatusCodes.Status404NotFound);
    }

    var result = await _userManager.ResetPasswordAsync(user, request.AccessToken, request.Password);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
      return Result<string>.Failure(errors, StatusCodes.Status500InternalServerError);
    }

    return Result<string>.Success(null, StatusCodes.Status200OK);
  }
}
