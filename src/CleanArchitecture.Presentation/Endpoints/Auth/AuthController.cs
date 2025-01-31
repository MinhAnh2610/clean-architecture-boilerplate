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
        ? Results.Ok(new ApiResponse<AuthResponse>(true, result.Data!, "Login Successfully.", null))
        : Results.BadRequest(new ApiResponse<AuthResponse>(false, null, "Login Failed.", result.Errors));
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
          ? Results.Ok(new ApiResponse<AuthResponse>(true, result.Data!, "Register Successfully.", null))
          : Results.BadRequest(new ApiResponse<AuthResponse>(false, null, "Register Failed.", result.Errors));
    })
    .WithName("Register")
    .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .WithSummary("Register")
    .WithDescription("User Registration");
    #endregion

    #region Forgot Password API
    group.MapPost("/forgot-password", async (IAuthService authService, ForgotPasswordRequest request) =>
    {
      var result = await authService.ForgotPasswordAsync(request);
      return result.IsSuccess
          ? Results.Ok(new ApiResponse<string>(true, result.Data!, "A Password Reset Request Has Been Sent To Your Email.", null))
          : Results.BadRequest(new ApiResponse<string>(false, null, "Failed To Request A Password Reset.", result.Errors));
    })
    .WithName("ForgotPassword")
    .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .WithSummary("ForgotPassword")
    .WithDescription("Enter Email To Reset Your Password");
    #endregion

    #region Reset Password API
    group.MapPost("/reset-password", async (IAuthService authService, ResetPasswordRequest request) =>
    {
      var result = await authService.ResetPasswordAsync(request);
      return result.IsSuccess
          ? Results.Ok(new ApiResponse<string>(true, result.Data!, "Password Has Been Successfully Changed.", null))
          : Results.BadRequest(new ApiResponse<string>(false, null, "Reset Password Failed.", result.Errors));
    })
    .WithName("ResetPassword")
    .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .WithSummary("ResetPassword")
    .WithDescription("Enter Your New Password To Change It");
    #endregion
  }
}
