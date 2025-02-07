using CleanArchitecture.Application.DTOs.Role;
using CleanArchitecture.Application.DTOs.User;
using CleanArchitecture.Application.Enums;

namespace CleanArchitecture.Presentation.Endpoints;

public class RoleController : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("api/role").WithTags("Roles");

    #region Assign Role API
    group.MapPost("/assign-role", async (IRoleService roleService, AssignRoleRequest request) =>
    {
      var result = await roleService.AssignRoleAsync(request);
      if (result.IsSuccess)
      {
        return Results.Ok(ApiResponse<UserProfileResponse>.SuccessResponse(result.Data!, "Assign User To Roles Successfully."));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(ApiResponse<UserProfileResponse>.FailureResponse(result.Errors, "Input Validation Failed.")),
        StatusCodes.Status404NotFound => Results.BadRequest(ApiResponse<UserProfileResponse>.FailureResponse(result.Errors, "Resource Not Found.")),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError),
      };
    })
    .WithName("AssignRole")
    .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("AssignRole")
    .WithDescription("Assign User To Roles")
    .RequireAuthorization(new AuthorizeAttribute
    {
      Roles = Roles.Admin
    });
    #endregion
  }
}
