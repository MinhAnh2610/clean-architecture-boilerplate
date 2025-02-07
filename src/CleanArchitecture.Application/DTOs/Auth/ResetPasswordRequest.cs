namespace CleanArchitecture.Application.DTOs.Auth;

public record ResetPasswordRequest(string AccessToken, string Email, string Password, string PasswordConfirmation);
