using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.ServiceContracts;

public interface ITokenService
{
  Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken);
  Task<Result<string>> RevokeRefreshTokenAsync(string refreshToken);
}
