using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.ServiceContracts;

public interface IAuthService
{
  Task<Result<AuthResponse>> LoginAsync(LoginRequest loginRequest);
  Task<Result<AuthResponse>> RegisterAsync(RegisterRequest registerRequest);
  Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest);
  Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
}
