using CleanArchitecture.Application.DTOs.Email;
using CleanArchitecture.Application.ServiceContracts;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application.Validators;
using CleanArchitecture.Application.Validators.Auth;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace CleanArchitecture.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplicationServices
    (this IServiceCollection services, IConfiguration configuration)
  {

    services.AddFeatureManagement();
    services.AddHttpContextAccessor();

    // Add validators
    services.AddValidatorsFromAssemblyContaining<LoginValidator>();

    // Add Email Configuration
    services.AddSingleton(configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>()!);

    // Add identity server 4 validator for owner password
    services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();

    // Add services
    services.AddScoped<IAuthService, AuthService>();
    //services.AddScoped<IProfileService, ProfileService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IRoleService, RoleService>();
    services.AddScoped<ITimeZoneService, TimeZoneService>();

    return services;
  }
}
