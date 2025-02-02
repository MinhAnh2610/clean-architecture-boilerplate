using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CleanArchitecture.Presentation.Middlewares;

public class CustomErrorHandlingMiddleware : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    var response = httpContext.Response;
    response.ContentType = "application/json";
    response.StatusCode = StatusCodes.Status500InternalServerError;

    var apiResponse = ApiResponse<string>.FailureResponse(
        new List<Error>(),
        "An Unexpected Error Occurred On The Server."
    );

    await response.WriteAsJsonAsync(apiResponse, cancellationToken);

    return true; // Mark the exception as handled
  }
}
