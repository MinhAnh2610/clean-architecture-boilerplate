namespace CleanArchitecture.Application.DTOs.Auth;

public record RegisterRequest(
  string UserName, 
  string Email,
  bool Gender,
  string PhoneNumber,
  string FirstName,
  string LastName,
  string Password, 
  string PasswordConfirmation);
