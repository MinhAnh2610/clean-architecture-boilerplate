using Microsoft.AspNetCore.Http;
using System.Text;

namespace CleanArchitecture.Application.Common;

/// <summary>
/// This class implement result pattern to avoid using exception as a flow (because exception are expensive).
/// </summary>
/// <typeparam name="T">Type of the result</typeparam>
public class Result<T>
{
  private Result(bool isSuccess, List<Error> errors, T? data, int status)
  {
    if (isSuccess && errors.Count > 0 ||
        !isSuccess && errors.Count == 0)
    {
      throw new ArgumentException("Invalid error", nameof(errors));
    }

    IsSuccess = isSuccess;
    Errors = errors;
    Data = data;
    Status = status;
  }

  public bool IsSuccess { get; }

  public bool IsFailure => !IsSuccess;

  public List<Error> Errors { get; }

  public T? Data { get; }

  public int Status { get; }

  /// <summary>
  /// This function create a Result with type T data and isSuccess is set to true.
  /// </summary>
  /// <param name="data">Data of the result.</param>
  /// <param name="status">Status code of the Result.</param>
  /// <returns></returns>
  public static Result<T> Success(T data, int status) => new Result<T>(true, new List<Error>(), data, status);


  /// <summary>
  /// This function create a Result with a list of errors and isSuccess is set to false.
  /// </summary>
  /// <param name="errors">The list of data with error type.</param>
  /// <param name="status">Status code of the Result.</param>
  /// <returns></returns>
  public static Result<T> Failure(List<Error> errors, int status) => new Result<T>(false, errors, default, status);

  /// <summary>
  /// This function returns the Results of the API according to the success of the Result, the status code of the Result, and its errors
  /// </summary>
  /// <param name="The message used if the result is success, otherwise this message won't be used."></param>
  /// <returns>API Results</returns>
  public IResult Match(string message)
  {
    if (Errors.Count > 0)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (var error in Errors)
      {
        stringBuilder.AppendLine(error.Description);
      }

      message = stringBuilder.ToString();
    }

    var response = IsSuccess
      ? ApiResponse<T>.SuccessResponse(Data, message)
      : ApiResponse<T>.FailureResponse(Errors, message);
    return Status switch
    {
      StatusCodes.Status200OK => Results.Ok(response),
      StatusCodes.Status400BadRequest => Results.BadRequest(response),
      StatusCodes.Status401Unauthorized => Results.Unauthorized(),
      StatusCodes.Status409Conflict => Results.Conflict(response),
      StatusCodes.Status404NotFound => Results.NotFound(response),
      StatusCodes.Status201Created => Results.Created(),
      _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
    };
  }
}