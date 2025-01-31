using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CleanArchitecture.Presentation;

public static class DependencyInjection
{
  public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
  {
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

    // Add identity
    services.AddIdentity<User, Role>(options =>
    {
      options.Password.RequiredLength = 5;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequireUppercase = false;
      options.Password.RequireLowercase = false;
      options.Password.RequireDigit = false;
      options.Password.RequiredUniqueChars = 0;

      options.Lockout.AllowedForNewUsers = false;
    })
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders()
      .AddUserStore<UserStore<User, Role, ApplicationDbContext>>()
      .AddRoleStore<RoleStore<Role, ApplicationDbContext>>();

    // Add authentication & authorization
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
        options.Authority = "https://localhost:5051";
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateAudience = false,
        };
      });

    services.AddAuthorization(options =>
    {
      options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "client"));
    });

    services.AddCarter();

    //services.AddExceptionHandler<CustomExceptionHandler>();

    //services.AddHealthChecks().AddNpgSql(config.GetConnectionString("Database")!);

    return services;
  }

  public static WebApplication UseApiServices(this WebApplication app)
  {
    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseIdentityServer();

    app.UseAuthentication();
    app.UseAuthorization();

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
