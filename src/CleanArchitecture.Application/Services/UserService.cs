using CleanArchitecture.Application.DTOs.User;
using CleanArchitecture.Application.ServiceContracts;
using FluentValidation;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CleanArchitecture.Application.Services;

public class UserService : IUserService
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly UserManager<User> _userManager;
  private readonly IValidator<UpdateProfileRequest> _updateProfileValidator;
  private readonly IValidator<UserRequest> _userValidator;

  public UserService(IHttpContextAccessor httpContextAccessor,
                     UserManager<User> userManager,
                     IValidator<UpdateProfileRequest> updateProfileValidator,
                     IValidator<UserRequest> userValidator)
  {
    _httpContextAccessor = httpContextAccessor;
    _userManager = userManager;
    _updateProfileValidator = updateProfileValidator;
    _userValidator = userValidator;
  }

  public async Task<Result<string>> DisableUserAsync(UserRequest request)
  {
    var validationResult = await _userValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<string>.Failure(errors, StatusCodes.Status400BadRequest);
    }

    var user = await _userManager.FindByNameAsync(request.UserName);
    if (user == null)
      return Result<string>.Failure([AuthErrors.UserNotFound], StatusCodes.Status404NotFound);
    user.RefreshToken = null;
    user.RefreshTokenExpiration = null;

    user.LockoutEnabled = true;
    user.LockoutEnd = DateTime.UtcNow.AddYears(100);
    var result = await _userManager.UpdateAsync(user);
    var resultErrors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
    return result.Succeeded
      ? Result<string>.Success(default!, StatusCodes.Status200OK)
      : Result<string>.Failure(resultErrors, StatusCodes.Status400BadRequest);
  }

  public async Task<Result<string>> EnableUserAsync(UserRequest request)
  {
    var validationResult = await _userValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<string>.Failure(errors, StatusCodes.Status400BadRequest);
    }

    var user = await _userManager.FindByNameAsync(request.UserName);
    if (user == null)
      return Result<string>.Failure([AuthErrors.UserNotFound], StatusCodes.Status404NotFound);

    user.LockoutEnabled = false;
    var result = await _userManager.UpdateAsync(user);
    var resultErrors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
    return result.Succeeded
      ? Result<string>.Success(default!, StatusCodes.Status200OK)
      : Result<string>.Failure(resultErrors, StatusCodes.Status400BadRequest);
  }

  public async Task<Result<UserProfileResponse>> GetUserProfile()
  {
    var user = _httpContextAccessor.HttpContext?.User;
    if (user == null || !user.Identity!.IsAuthenticated)
    {
      return Result<UserProfileResponse>.Failure([UserErrors.UnauthorizedUser], StatusCodes.Status401Unauthorized);
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
    }, StatusCodes.Status200OK);
  }

  public async Task<Result<UserProfileResponse>> UpdateUserProfileAsync(UpdateProfileRequest request)
  {
    var validationResult = await _updateProfileValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<UserProfileResponse>.Failure(errors, StatusCodes.Status400BadRequest);
    }

    var authorizedUser = _httpContextAccessor.HttpContext?.User;
    if (authorizedUser == null || !authorizedUser.Identity!.IsAuthenticated)
    {
      return Result<UserProfileResponse>.Failure([UserErrors.UnauthorizedUser], StatusCodes.Status401Unauthorized);
    }
    var id = authorizedUser.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    var roles = authorizedUser.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
      return Result<UserProfileResponse>.Failure([AuthErrors.UserNotFound], StatusCodes.Status404NotFound);

    user.UserName = request.UserName ?? user.UserName;
    var duplicateUser = await _userManager.FindByNameAsync(user.UserName!);
    if (duplicateUser != null && duplicateUser.Id != user.Id)
      return Result<UserProfileResponse>.Failure([AuthErrors.DuplicateUserName], StatusCodes.Status409Conflict);

    user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
    user.BirthDate = request.BirthDate ?? user.BirthDate;
    user.FirstName = request.FirstName ?? user.FirstName;
    user.LastName = request.LastName ?? user.LastName;
    user.Gender = request.Gender ?? user.Gender;

    var result = await _userManager.UpdateAsync(user);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
      return Result<UserProfileResponse>.Failure(errors, StatusCodes.Status500InternalServerError);
    }

    return Result<UserProfileResponse>.Success(new UserProfileResponse
    {
      Id = id,
      UserName = user.UserName!,
      Email = user.Email!,
      PhoneNumber = user.PhoneNumber,
      BirthDate = user.BirthDate,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Gender = user.Gender,
      Roles = roles
    }, StatusCodes.Status200OK);
  }
}
