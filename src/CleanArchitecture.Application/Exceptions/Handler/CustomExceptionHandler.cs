using CleanArchitecture.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CleanArchitecture.Application.Exceptions.Handler;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
  {
    logger.LogError("Error Message: {exceptionMessage}, Time of occurence {time}",
      exception.Message, DateTime.UtcNow);

    // Determine the status code and error details based on exception type
    (string Message, List<string> Errors, int StatusCode) details = exception switch
    {
      ValidationException validationException =>
      (
          validationException.Message,
          validationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList(),
          StatusCodes.Status400BadRequest
      ),
      BadRequestException badRequestException =>
      (
          badRequestException.Message,
          new List<string> { badRequestException.Message },
          StatusCodes.Status400BadRequest
      ),
      NotFoundException notFoundException =>
      (
          notFoundException.Message,
          new List<string> { notFoundException.Message },
          StatusCodes.Status404NotFound
      ),
      InternalServerException internalServerException =>
      (
          internalServerException.Message,
          new List<string> { internalServerException.Message },
          StatusCodes.Status500InternalServerError
      ),
      _ =>
      (
          exception.Message,
          new List<string> { exception.Message },
          StatusCodes.Status500InternalServerError
      )
    };

    // Create a wrapped API response
    var response = new ApiResponse<object>(
        message: details.Message,
        errors: details.Errors
    )
    {
      Success = false
    };

    // Set the HTTP response status code
    context.Response.StatusCode = details.StatusCode;
    context.Response.ContentType = "application/json";

    // Include traceId and additional debugging info (optional)
    response.Data = new
    {
      TraceId = context.TraceIdentifier,
      Path = context.Request.Path
    };

    // Serialize and write the response
    var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    await context.Response.WriteAsync(jsonResponse, cancellationToken);

    return true;
  }
}
