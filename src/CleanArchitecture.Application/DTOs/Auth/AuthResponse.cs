namespace CleanArchitecture.Application.DTOs.Auth;

public class AuthResponse
{
  public string UserName { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string AccessToken { get; set; } = default!;
  public int AccessTokenExpiration { get; set; }
  public string RefreshToken { get; set; } = default!;
  public int RefreshTokenExpiration { get; set; }
}

