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
        ClientName = "Web API",
        AllowedGrantTypes = GrantTypes.Hybrid,
        RequirePkce = false,
        AllowRememberConsent = false,
        RedirectUris = new List<string>()
        {
          "https://localhost:5002/signin-oidc"
        },
        PostLogoutRedirectUris = new List<string>()
        {
          "https://localhost:5002/signout-callback-oidc"
        },
        ClientSecrets = new List<Secret>
        {
          new Secret("secret".Sha256()),
        },
        AllowedScopes = new List<string>()
        {
          IdentityServerConstants.StandardScopes.OpenId,
          IdentityServerConstants.StandardScopes.Profile,
          IdentityServerConstants.StandardScopes.Address,
          IdentityServerConstants.StandardScopes.Email,
          "API",
          "roles"
        }
      }
    };
  public static IEnumerable<ApiScope> ApiScopes =>
    new ApiScope[]
    {
      new ApiScope("API", "Web API")
    };
  public static IEnumerable<ApiResource> ApiResources => new ApiResource[] { };
  public static IEnumerable<IdentityResource> IdentityResources =>
    new IdentityResource[]
    {
      new IdentityResources.OpenId(),
      new IdentityResources.Profile(),
      new IdentityResources.Address(),
      new IdentityResources.Email(),
      new IdentityResource(
        "roles",
        "Your role(s)",
        new List<string>() { "role" })
    };
  public static List<TestUser> TestUsers =>
    new List<TestUser>
    {
    };
}
