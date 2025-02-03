using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.ServiceContracts;

public interface IAuthService
{
  Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
  Task<Result<string>> RegisterAsync(RegisterRequest request);
  Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
  Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request);
  Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
}
