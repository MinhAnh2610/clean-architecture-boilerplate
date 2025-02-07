using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Data.Extensions;

public static class DatabaseExtensions
{
  public static async Task InitializeDatabaseAsync(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    //context.Database.MigrateAsync().GetAwaiter().GetResult();

    await SeedAsync(context, roleManager, userManager);
  }

  private static async Task SeedAsync(ApplicationDbContext context, RoleManager<Role> roleManager, UserManager<User> userManager)
  {
    await SeedRoleAsync(context, roleManager);
    await SeedUserAsync(context, userManager);
  }

  private static async Task SeedRoleAsync(ApplicationDbContext context, RoleManager<Role> roleManager)
  {
    if (!await context.Roles.AnyAsync())
    {
      //await context.Roles.AddRangeAsync(InitialData.Roles);
      //await context.SaveChangesAsync();
      foreach (var role in InitialData.Roles)
      {
        await roleManager.CreateAsync(role);
      }
    }
  }

  private static async Task SeedUserAsync(ApplicationDbContext context, UserManager<User> userManager)
  {
    if (!await context.Users.AnyAsync())
    {
      //await context.Users.AddRangeAsync(InitialData.Users);
      //await context.SaveChangesAsync();
      foreach (var user in InitialData.Users)
      {
        await userManager.CreateAsync(user, "12345");
        await userManager.AddToRolesAsync(user, ["Admin", "Customer"]);
      }
    }
  }
}
