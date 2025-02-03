using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.Enums;

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
        return Results.Ok(ApiResponse<AuthResponse>.SuccessResponse(result.Data!, "Login Successfully."));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(ApiResponse<AuthResponse>.FailureResponse(result.Errors, "Input Validation Failed.")),
        StatusCodes.Status423Locked => Results.BadRequest(ApiResponse<AuthResponse>.FailureResponse(result.Errors, "The Resource Being Requested Is Locked.")),
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
        return Results.Ok(ApiResponse<string>.SuccessResponse(result.Data!, "Register Successfully."));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(ApiResponse<string>.FailureResponse(result.Errors, "Input Validation Failed.")),
        StatusCodes.Status409Conflict => Results.Conflict(ApiResponse<string>.FailureResponse(result.Errors, "Resource Already Exists.")),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("Register")
    .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
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
        return Results.Ok(ApiResponse<string>.SuccessResponse(result.Data!, "A Password Reset Request Has Been Sent To Your Email."));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(ApiResponse<string>.FailureResponse(result.Errors, "Input Validation Failed.")),
        StatusCodes.Status404NotFound => Results.NotFound(ApiResponse<string>.FailureResponse(result.Errors, "Resource Not Found")),
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
        return Results.Ok(ApiResponse<string>.SuccessResponse(result.Data!, "Password Has Been Changed Successfully."));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(ApiResponse<string>.FailureResponse(result.Errors, "Input Validation Failed.")),
        StatusCodes.Status404NotFound => Results.NotFound(ApiResponse<string>.FailureResponse(result.Errors, "Resource Not Found")),
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

    #region Refresh Token API
    group.MapPost("/refresh-token", async (IAuthService authService, RefreshTokenRequest request) =>
    {
      var result = await authService.RefreshTokenAsync(request);
      if (result.IsSuccess)
      {
        return Results.Ok(ApiResponse<AuthResponse>.SuccessResponse(result.Data!, "Token Has Been Refreshed Successfully."));
      }

      return result.Status switch
      {
        StatusCodes.Status400BadRequest => Results.BadRequest(ApiResponse<string>.FailureResponse(result.Errors, "Input Validation Failed.")),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
      };
    })
    .WithName("RefreshToken")
    .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithSummary("RefreshToken")
    .WithDescription("Enter Your Refresh Token To Refresh The Access Token")
    .RequireAuthorization(new AuthorizeAttribute
    {
      AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    });
    #endregion
  }
}
