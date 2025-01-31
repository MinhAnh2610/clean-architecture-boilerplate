namespace CleanArchitecture.Application.Exceptions;

public static class UserErrors
{
  public static readonly Error UnauthorizedUser = new Error("UserErrors.UnauthorizedUser", "User is not auhenticated.");

  public static Error TokenResponseError(string tokenError) => new Error("UserErrors.TokenResponseError", $"Token error: {tokenError}.");

  public static Error UserInfoResponseError(string userInfoError) => new Error("UserErrors.UserInfoResponseError", $"User info error: {userInfoError}.");
}
