using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace CleanArchitecture.Presentation;

public static class DependencyInjection
{
  public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
  {
    services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
      options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
      {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
      });
      options.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
            {
              Type = ReferenceType.SecurityScheme,
              Id = "Bearer"
            }
          },
          new string[]{}
        }
      });
    });

    services.AddCarter();

    //services.AddExceptionHandler<CustomExceptionHandler>();

    //services.AddHealthChecks().AddNpgSql(config.GetConnectionString("Database")!);

    return services;
  }

  public static WebApplication UseApiServices(this WebApplication app)
  {
    app.MapCarter();

    //app.UseExceptionHandler(options => { });
    //app.UseHealthChecks("health",
    //  new HealthCheckOptions
    //  {
    //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    //  });

    return app;
  }
}
