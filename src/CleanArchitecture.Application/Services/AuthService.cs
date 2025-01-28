using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Services;

public class AuthService : IAuthService
{
  private readonly UserManager<User> _userManager;
  private readonly IHttpClientFactory _httpClientFactory;

  public AuthService(UserManager<User> userManager, IHttpClientFactory httpClientFactory)
  {
    _userManager = userManager;
    _httpClientFactory = httpClientFactory;
  }

  public async Task<Result> LoginAsync(LoginRequest loginRequest)
  {
    var user = await _userManager.FindByEmailAsync(loginRequest.Email);
    if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
    {
      //return Result.Failure();
    }
    throw new NotImplementedException();
  }

  public Task<Result> RegisterAsync(RegisterRequest registerRequest)
  {
    throw new NotImplementedException();
  }
}
