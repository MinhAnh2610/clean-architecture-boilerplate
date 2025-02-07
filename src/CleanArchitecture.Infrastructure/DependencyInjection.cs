using CleanArchitecture.Application.Services;
using CleanArchitecture.Domain.RepositoryContracts.Base;
using CleanArchitecture.Domain.RepositoryContracts.UnitOfWork;
using CleanArchitecture.Infrastructure.Auth;
using CleanArchitecture.Infrastructure.Data.Interceptors;
using CleanArchitecture.Infrastructure.Redis;
using CleanArchitecture.Infrastructure.Repositories.Base;
using CleanArchitecture.Infrastructure.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    // Add services to the container
    services.AddIdentityServer()
            //.AddAspNetIdentity<User>()
            .AddInMemoryApiScopes(Config.ApiScopes)      // Define API scopes
            .AddInMemoryApiResources(Config.ApiResources) // Define API resources
            .AddInMemoryClients(Config.Clients)          // Define clients
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddDeveloperSigningCredential()            // Use for dev, use a real certificate in prod
            .AddProfileService<ProfileService>();

    services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

    services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
    {
      options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
      //options.UseNpgsql(configuration.GetConnectionString("Database"));
      options.UseInMemoryDatabase("Database");
    });

    services.AddStackExchangeRedisCache(options =>
    {
      options.Configuration = configuration.GetConnectionString("Redis");
    });

    services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

    // Register Unit of Work
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Register Generic Repository
    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

    // Register Redis Caching
    services.AddScoped<IRedisCacheRepository, RedisCacheRepository>();

    return services;
  }
}
