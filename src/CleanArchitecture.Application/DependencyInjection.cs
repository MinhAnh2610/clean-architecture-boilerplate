using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.DTOs.Role;
using CleanArchitecture.Application.DTOs.User;
using CleanArchitecture.Application.ServiceContracts;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application.Validators;
using CleanArchitecture.Application.Validators.Auth;
using CleanArchitecture.Application.Validators.Role;
using CleanArchitecture.Application.Validators.User;
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
    #region Auth Validators
    services.AddScoped<IValidator<LoginRequest>, LoginValidator>();
    services.AddScoped<IValidator<RegisterRequest>, RegisterValidator>();
    services.AddScoped<IValidator<ForgotPasswordRequest>, ForgotPasswordValidator>();
    services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordValidator>();
    services.AddScoped<IValidator<RefreshTokenRequest>, RefreshTokenValidator>();
    #endregion

    #region User Validators
    services.AddScoped<IValidator<UpdateProfileRequest>, UpdateProfileValidator>();
    services.AddScoped<IValidator<UserRequest>, UserValidator>();
    #endregion

    #region Role Validators
    services.AddScoped<IValidator<AssignRoleRequest>, AssignRoleValidator>();
    #endregion

    // Add identity server 4 validator for owner password
    services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();

    // Add services
    services.AddScoped<IAuthService, AuthService>();
    //services.AddScoped<IProfileService, ProfileService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IRoleService, RoleService>();

    return services;
  }
}
