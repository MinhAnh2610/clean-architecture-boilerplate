namespace CleanArchitecture.Infrastructure.Data.Extensions;

internal class InitialData
{
  public static IEnumerable<User> Users =>
    new List<User>
    {
      new() { UserName = "Admin123", Email = "admin123@gmail.com", EmailConfirmed = true }
    };

  public static IEnumerable<Role> Roles =>
    new List<Role>
    {
      new() { Name = "Admin" },
      new() { Name = "Customer" }
    };
}
