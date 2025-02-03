using CleanArchitecture.Application.DTOs.Role;
using CleanArchitecture.Application.DTOs.User;
using CleanArchitecture.Application.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Services;

public class RoleService : IRoleService
{
  private readonly UserManager<User> _userManager;
  private readonly RoleManager<Role> _roleManager;
  private readonly IValidator<AssignRoleRequest> _assignRoleValidator;

  public RoleService(UserManager<User> userManager, RoleManager<Role> roleManager, IValidator<AssignRoleRequest> assignRoleValidator)
  {
    _roleManager = roleManager;
    _userManager = userManager;
    _assignRoleValidator = assignRoleValidator;
  }

  public async Task<Result<UserProfileResponse>> AssignRoleAsync(AssignRoleRequest request)
  {
    var validationResult = await _assignRoleValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error("ValidationError", e.ErrorMessage))
          .ToList();

      return Result<UserProfileResponse>.Failure(errors, StatusCodes.Status400BadRequest);
    }

    var user = await _userManager.FindByNameAsync(request.UserName);
    if (user == null)
      return Result<UserProfileResponse>.Failure([AuthErrors.UserNotFound], StatusCodes.Status404NotFound);

    var roleErrors = new List<Error>();
    foreach (var role in request.Roles)
    {
      if (!await _roleManager.RoleExistsAsync(role))
      {
        roleErrors.Add(RoleErrors.RoleNotFound(role));
      }
    }
    if (roleErrors.Any())
    {
      return Result<UserProfileResponse>.Failure(roleErrors, StatusCodes.Status400BadRequest);
    }

    var userRoles = await _userManager.GetRolesAsync(user);
    var result = await _userManager.RemoveFromRolesAsync(user, userRoles);
    result = await _userManager.AddToRolesAsync(user, request.Roles);
    var resultErrors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
    var userProfile = new UserProfileResponse
    {
      Id = user.Id,
      UserName = user.UserName!,
      Email = user.Email!,
      BirthDate = user.BirthDate,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Gender = user.Gender,
      PhoneNumber = user.PhoneNumber,
      Roles = request.Roles,
    };
    return result.Succeeded
      ? Result<UserProfileResponse>.Success(userProfile, StatusCodes.Status200OK)
      : Result<UserProfileResponse>.Failure(resultErrors, StatusCodes.Status400BadRequest);
  }
}
