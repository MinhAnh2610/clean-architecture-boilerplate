using CleanArchitecture.Application.DTOs.User;

namespace CleanArchitecture.Presentation.Endpoints.User;

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
        return Results.Ok(new ApiResponse<UserProfileResponse>(true, result.Data, "Retrieve User Profile Successfully.", result.Errors));
      }

      return result.Status switch
      {
        StatusCodes.Status401Unauthorized => Results.Unauthorized(),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("UserProfile")
    .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("UserProfile")
    .WithDescription("Get User Profile")
    .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme });
    #endregion

    #region Update User Profile API
    group.MapPut("/profile", async (IUserService userService, UpdateProfileRequest request) =>
    {
      var result = await userService.UpdateUserProfileAsync(request);
      if (result.IsSuccess)
      {
        Results.Ok(new ApiResponse<UserProfileResponse>(true, result.Data, "Update User Profile Successfully.", result.Errors));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(new ApiResponse<UserProfileResponse>(false, result.Data, "Input Validation Failed.", result.Errors)),
        StatusCodes.Status401Unauthorized => Results.Unauthorized(),
        StatusCodes.Status404NotFound => Results.NotFound(new ApiResponse<UserProfileResponse>(false, result.Data, "Resource Not Found.", result.Errors)),
        StatusCodes.Status409Conflict => Results.Conflict(new ApiResponse<UserProfileResponse>(false, result.Data, "Resource Already Exists", result.Errors)),
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
    .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme });
    #endregion
  }
}
