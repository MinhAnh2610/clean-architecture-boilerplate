using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.ServiceContracts;

public interface IAuthService
{
  Task<Result> LoginAsync(LoginRequest loginRequest);
  Task<Result> RegisterAsync(RegisterRequest registerRequest);
}
