namespace CleanArchitecture.Presentation.Endpoints.Admin;

[Authorize(Roles = "Admin")]
public class AccountController : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/users", async () =>
    {

    });
  }
}
