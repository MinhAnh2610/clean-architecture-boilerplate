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
      if (result.IsSuccess)
      {
        return Results.Ok(new ApiResponse<AuthResponse>(true, result.Data, "Login Successfully.", result.Errors));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(new ApiResponse<AuthResponse>(false, result.Data, "Input Validation Failed.", result.Errors)),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError),
      };
    })
    .WithName("Login")
    .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("Login")
    .WithDescription("User Login Authentication");
    #endregion

    #region Register API
    group.MapPost("/register", async (IAuthService authService, RegisterRequest request) =>
    {
      var result = await authService.RegisterAsync(request);
      if (result.IsSuccess)
      {
        return Results.Ok(new ApiResponse<AuthResponse>(true, result.Data, "Register Successfully.", null));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(new ApiResponse<AuthResponse>(false, result.Data, "Input Validation Failed.", result.Errors)),
        StatusCodes.Status409Conflict => Results.Conflict(new ApiResponse<AuthResponse>(false, result.Data, "Resource Already Exists.", result.Errors)),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("Register")
    .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("Register")
    .WithDescription("User Registration");
    #endregion

    #region Forgot Password API
    group.MapPost("/forgot-password", async (IAuthService authService, ForgotPasswordRequest request) =>
    {
      var result = await authService.ForgotPasswordAsync(request);
      if (result.IsSuccess)
      {
        return Results.Ok(new ApiResponse<string>(true, result.Data, "A Password Reset Request Has Been Sent To Your Email.", result.Errors));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(new ApiResponse<string>(false, result.Data, "Input Validation Failed.", result.Errors)),
        StatusCodes.Status404NotFound => Results.NotFound(new ApiResponse<string>(false, result.Data, "Resource Not Found", result.Errors)),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("ForgotPassword")
    .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("ForgotPassword")
    .WithDescription("Enter Email To Reset Your Password");
    #endregion

    #region Reset Password API
    group.MapPost("/reset-password", async (IAuthService authService, ResetPasswordRequest request) =>
    {
      var result = await authService.ResetPasswordAsync(request);
      if (result.IsSuccess)
      {
        return Results.Ok(new ApiResponse<string>(true, result.Data, "Password Has Been Changed Successfully.", result.Errors));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(new ApiResponse<string>(false, result.Data, "Input Validation Failed.", result.Errors)),
        StatusCodes.Status404NotFound => Results.NotFound(new ApiResponse<string>(false, result.Data, "Resource Not Found", result.Errors)),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("ResetPassword")
    .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("ResetPassword")
    .WithDescription("Enter Your New Password To Change It");
    #endregion
  }
}
