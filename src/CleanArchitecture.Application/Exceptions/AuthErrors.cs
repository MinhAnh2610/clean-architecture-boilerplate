namespace CleanArchitecture.Application.Exceptions;

public static class AuthErrors
{
  public static readonly Error InvalidCredentials = new Error("AuthErrors.InvalidCredentials", "Wrong email or password, pls try again.");

  public static readonly Error NotRegistered = new Error("AuthErrors.NotRegistered", "You haven't sign up yet.");

  public static readonly Error IdentityServerFailed = new Error("AuthErrors.IdentityServerFailed", "Failed to fetch discovery document.");

  public static Error TokenResponseError(string tokenError) => new Error("AuthErrors.TokenResponseError", $"Token error: {tokenError}.");
}
