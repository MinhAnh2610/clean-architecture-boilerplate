using CleanArchitecture.Application.DTOs.User;
using CleanArchitecture.Application.ServiceContracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
      return result.IsSuccess
        ? Results.Ok(new ApiResponse<UserProfileResponse>(true, result.Data!, "Retrieve User Profile Successfully.", null))
        : Results.BadRequest(new ApiResponse<UserProfileResponse>(false, null, "Retrieve User Profile Failed.", result.Errors));
    })
    .WithName("UserProfile")
    .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .WithSummary("UserProfile")
    .WithDescription("Get User Profile")
    .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme });
    #endregion
  }
}
