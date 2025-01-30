using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.ServiceContracts;

namespace CleanArchitecture.Presentation.Endpoints.Auth;

public class AuthController : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("api/auth").WithTags("Authentication");

    #region Login API
    group.MapPost("/login", async (IAuthService authService, LoginRequest request) =>
    {
      var result = await authService.LoginAsync(request);
      return result.IsSuccess
        ? Results.Ok(new ApiResponse<AuthResponse>(true, result.Data!, "Login Successfully", null))
        : Results.BadRequest(new ApiResponse<AuthResponse>(false, null, "Login Failed", result.Errors));
    })
    .WithName("Login")
    .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .WithSummary("Login")
    .WithDescription("User Login Authentication");
    #endregion

    #region Register API
    group.MapPost("/register", async (IAuthService authService, RegisterRequest request) =>
    {
      var result = await authService.RegisterAsync(request);
      return result.IsSuccess
          ? Results.Ok(new ApiResponse<AuthResponse>(true, result.Data!, "Register Successfully", null))
          : Results.BadRequest(new ApiResponse<AuthResponse>(false, null, "Register Failed", result.Errors));
    })
    .WithName("Register")
    .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .WithSummary("Register")
    .WithDescription("User Registration");
    #endregion
    
  }
}
