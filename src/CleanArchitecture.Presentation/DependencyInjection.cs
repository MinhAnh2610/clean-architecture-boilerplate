using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.Google;
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

      options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
      options.Lockout.MaxFailedAccessAttempts = 5;
      options.Lockout.AllowedForNewUsers = true;
    })
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders()
      .AddUserStore<UserStore<User, Role, ApplicationDbContext>>()
      .AddRoleStore<RoleStore<Role, ApplicationDbContext>>();

    // Add authentication & authorization
    services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
      .AddCookie()
      .AddJwtBearer(options =>
      {
        options.Authority = "https://localhost:5051";
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateAudience = false,
        };
      })
      .AddGoogle(options =>
      {
        options.ClientId = config["Authentication:Google:ClientId"]!;
        options.ClientSecret = config["Authentication:Google:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
      });

    services.AddAuthorization(options =>
    {
      options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "client"));
    });

    services.AddCarter();

    //services.AddHealthChecks().AddNpgSql(config.GetConnectionString("Database")!);

    return services;
  }

  public static WebApplication UseApiServices(this WebApplication app)
  {
    app.UseExceptionHandler(options => { });
    app.UseMiddleware<CustomErrorHandler>();

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseIdentityServer();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapCarter();

    //app.UseHealthChecks("health",
    //  new HealthCheckOptions
    //  {
    //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    //  });

    return app;
  }
}
