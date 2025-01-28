using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Data.Extensions;

public static class DatabaseExtensions
{
  public static async Task InitializeDatabaseAsync(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    context.Database.MigrateAsync().GetAwaiter().GetResult();

    await SeedAsync(context);
  }

  private static async Task SeedAsync(ApplicationDbContext context)
  {
    //await SeedCustomerAsync(context);
    //await SeedProductAsync(context);
    //await SeedOrdersWithItemsAsync(context);
  }
}
