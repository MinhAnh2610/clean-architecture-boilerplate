namespace CleanArchitecture.Application.DTOs.Auth;

public record AuthResponse(
  string UserName,
  string Email,
  string AccessToken,
  DateTime AccessTokenExpiration,
  string RefreshToken,
  DateTime RefreshTokenExpiration
  );
