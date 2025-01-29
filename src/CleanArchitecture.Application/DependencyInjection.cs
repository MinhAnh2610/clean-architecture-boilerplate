using CleanArchitecture.Application.ServiceContracts;
using CleanArchitecture.Application.Services;
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


    // Add services
    services.AddScoped<IAuthService, AuthService>();
    //services.AddScoped<IUserService, UserService>();

    return services;
  }
}
