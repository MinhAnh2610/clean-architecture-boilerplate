namespace CleanArchitecture.Application.Exceptions;

public static class RoleErrors
{
  public static Error RoleNotFound(string role) => new Error("RoleErrors.RoleNotFound", $"Couldn't find the role {role}.");
}
