namespace CleanArchitecture.Infrastructure.Data.Extensions;

internal class InitialData
{
  public static IEnumerable<User> Users =>
    new List<User>
    {
      new() 
      { 
        Id = Guid.NewGuid().ToString(), 
        UserName = "Admin123", 
        Email = "admin123@gmail.com", 
        EmailConfirmed = true,
        BirthDate = new DateOnly(2000, 1, 1),
        FirstName = "John",
        LastName = "Doe",
        Gender = true,
        PhoneNumber = "1234567890"
      }
    };

  public static IEnumerable<Role> Roles =>
    new List<Role>
    {
      new() { Id = Guid.NewGuid().ToString(), Name = "Admin" },
      new() { Id = Guid.NewGuid().ToString(), Name = "Customer" }
    };
}
