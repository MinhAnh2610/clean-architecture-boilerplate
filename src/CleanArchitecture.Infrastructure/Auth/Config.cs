using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace CleanArchitecture.Infrastructure.Auth;

public class Config
{
  public static IEnumerable<Client> Clients =>
    new Client[]
    {
      new Client
      {
        ClientId = "api_client",
        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
        ClientSecrets =
        {
          new Secret("secret".Sha256())
        },
        AllowedScopes = new List<string>()
        {
          IdentityServerConstants.StandardScopes.OpenId,
          IdentityServerConstants.StandardScopes.Profile,
          IdentityServerConstants.StandardScopes.Address,
          IdentityServerConstants.StandardScopes.Email,
          "API",
          "roles"
        },
        AllowOfflineAccess = true,
        AccessTokenLifetime = 3600,
        RefreshTokenUsage = TokenUsage.ReUse,
        RefreshTokenExpiration = TokenExpiration.Sliding,
        AbsoluteRefreshTokenLifetime = 2592000, // 30 days
        SlidingRefreshTokenLifetime = 1296000, // 15 days
      }
    };
  public static IEnumerable<ApiScope> ApiScopes =>
    new ApiScope[]
    {
      new ApiScope("API",
        new[]
        {
          JwtClaimTypes.Subject,
          JwtClaimTypes.Profile,
          JwtClaimTypes.Address,
          JwtClaimTypes.Email,
          JwtClaimTypes.Name,
          JwtClaimTypes.Roles
        }),
      new ApiScope(IdentityServerConstants.StandardScopes.OfflineAccess) // Enable refresh tokens
    };
  public static IEnumerable<ApiResource> ApiResources =>
    new ApiResource[]
    {
      new ApiResource("API")
      {
        Scopes = { "API" },
        UserClaims = 
        { 
          JwtClaimTypes.Subject,
          JwtClaimTypes.Profile,
          JwtClaimTypes.Address,
          JwtClaimTypes.Email,
          JwtClaimTypes.Name,
          JwtClaimTypes.Roles 
        }
      }
    };
  public static IEnumerable<IdentityResource> IdentityResources =>
    new IdentityResource[]
    {
      new IdentityResources.OpenId(),
      new IdentityResources.Profile(),
      new IdentityResources.Address(),
      new IdentityResources.Email(),
      new IdentityResource("roles", new[] { JwtClaimTypes.Roles }),
      new IdentityResource("username", new[] { JwtClaimTypes.Name })
    };
  public static List<TestUser> TestUsers =>
    new List<TestUser>
    {
    };
}
