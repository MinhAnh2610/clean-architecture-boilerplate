namespace CleanArchitecture.Application.DTOs.User;

public record UpdateProfileRequest(
  string? UserName,
  string? PhoneNumber,
  DateOnly? BirthDate,
  string? FirstName,
  string? LastName,
  bool? Gender
  );
