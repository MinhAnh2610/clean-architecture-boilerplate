using CleanArchitecture.Application.DTOs.User;
using CleanArchitecture.Application.Enums;

namespace CleanArchitecture.Presentation.Endpoints;

public class UserController : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("api/user").WithTags("User Management");

    #region User Profile API
    group.MapGet("/profile", async (IUserService userService) =>
    {
      var result = await userService.GetUserProfile();
      if (result.IsSuccess)
      {
        return Results.Ok(ApiResponse<UserProfileResponse>.SuccessResponse(result.Data!, "Retrieve User Profile Successfully."));
      }

      return result.Status switch
      {
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("UserProfile")
    .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("UserProfile")
    .WithDescription("Get User Profile")
    .RequireAuthorization();
    #endregion

    #region Update User Profile API
    group.MapPut("/profile", async (IUserService userService, UpdateProfileRequest request) =>
    {
      var result = await userService.UpdateUserProfileAsync(request);
      if (result.IsSuccess)
      {
        Results.Ok(ApiResponse<UserProfileResponse>.SuccessResponse(result.Data!, "Update User Profile Successfully."));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(ApiResponse<UserProfileResponse>.FailureResponse(result.Errors, "Input Validation Failed.")),
        StatusCodes.Status401Unauthorized => Results.Unauthorized(),
        StatusCodes.Status404NotFound => Results.NotFound(ApiResponse<UserProfileResponse>.FailureResponse(result.Errors, "Resource Not Found.")),
        StatusCodes.Status409Conflict => Results.Conflict(ApiResponse<UserProfileResponse>.FailureResponse(result.Errors, "Resource Already Exists")),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("UpdateProfile")
    .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("UpdateProfile")
    .WithDescription("Update User Profile")
    .RequireAuthorization();
    #endregion

    #region Enable User API
    group.MapPut("/enable", async (IUserService userService, UserRequest request) =>
    {
      var result = await userService.EnableUserAsync(request);
      if (result.IsSuccess)
      {
        return Results.Ok(ApiResponse<string>.SuccessResponse(result.Data!, "User Enabled."));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(ApiResponse<string>.FailureResponse(result.Errors, "Input Validation Failed.")),
        StatusCodes.Status404NotFound => Results.BadRequest(ApiResponse<string>.FailureResponse(result.Errors, "Resource Not Found.")),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("EnableUser")
    .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("EnableUser")
    .WithDescription("Enable User Account")
    .RequireAuthorization(new AuthorizeAttribute
    {
      Roles = Roles.Admin,
    });
    #endregion

    #region Disable User API
    group.MapPut("/disable", async (IUserService userService, UserRequest request) =>
    {
      var result = await userService.DisableUserAsync(request);
      if (result.IsSuccess)
      {
        return Results.Ok(ApiResponse<string>.SuccessResponse(result.Data!, "User Disabled."));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(ApiResponse<string>.FailureResponse(result.Errors, "Input Validation Failed.")),
        StatusCodes.Status404NotFound => Results.BadRequest(ApiResponse<string>.FailureResponse(result.Errors, "Resource Not Found.")),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("DisableUser")
    .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("DisableUser")
    .WithDescription("Disable User Account")
    .RequireAuthorization(new AuthorizeAttribute
    {
      Roles = Roles.Admin,
    });
    #endregion
  }
}
