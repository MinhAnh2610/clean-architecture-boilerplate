using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace CleanArchitecture.Presentation;

public static class DependencyInjection
{
  public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
  {
    services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddCarter();

    //services.AddExceptionHandler<CustomExceptionHandler>();
    services.AddHealthChecks().AddNpgSql(config.GetConnectionString("Database")!);

    return services;
  }

  public static WebApplication UseApiServices(this WebApplication app)
  {
    app.MapCarter();

    //app.UseExceptionHandler(options => { });
    app.UseHealthChecks("health",
      new HealthCheckOptions
      {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
      });

    return app;
  }
}
