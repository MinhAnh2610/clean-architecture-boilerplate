namespace CleanArchitecture.Application.Exceptions;

public static class AuthErrors
{
  public static readonly Error InvalidCredentials = new Error("AuthErrors.InvalidCredentials", "Wrong email or password, pls try again.");

  public static readonly Error NotRegistered = new Error("AuthErrors.NotRegistered", "You haven't sign up yet.");

  public static readonly Error UserNotFound = new Error("AuthErrors.UserNotFound", "We couldn't find your account, make sure you given the correct credentials.");

  public static readonly Error AlreadyRegistered = new Error("AuthErrors.AlreadyRegistered", "You have registered with this email.");

  public static readonly Error DuplicateUserName = new Error("AuthErrors.DuplicateUserName", "This username has already been taken.");

  public static readonly Error RegistrationFailed = new Error("AuthErrors.RegistrationFailed", "Failed to register user, pls try again later.");

  public static readonly Error IdentityServerFailed = new Error("AuthErrors.IdentityServerFailed", "Failed to fetch discovery document.");

  public static Error TokenResponseError(string tokenError) => new Error("AuthErrors.TokenResponseError", $"Token error: {tokenError}.");
}
