using System.Net;
using System.Text.Json;

namespace CleanArchitecture.Presentation.Middlewares;

public class CustomErrorHandler
{
  private readonly RequestDelegate _next;

  public CustomErrorHandler(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    await _next(context);

    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
    {
      await HandleResponseAsync(context, HttpStatusCode.Unauthorized, "Authentication Required Or Failed.");
    }
    else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
      await HandleResponseAsync(context, HttpStatusCode.Forbidden, "You Do Not Have Permission To Access This Resource.");
    }
    else if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
    {
      await HandleResponseAsync(context, HttpStatusCode.InternalServerError, "An Unexpected Error Occurred On The Server.");
    }
  }

  private static async Task HandleResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
  {
    context.Response.ContentType = "application/json";

    var response = ApiResponse<string>.FailureResponse(
      new List<Error>(),
      message
      );

    var json = JsonSerializer.Serialize(response);
    await context.Response.WriteAsync(json);
  }
}
