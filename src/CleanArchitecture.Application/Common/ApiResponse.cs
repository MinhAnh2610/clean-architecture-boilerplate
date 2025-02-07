namespace CleanArchitecture.Application.Common;

public class ApiResponse<T>
{
  public ApiResponse(bool isSuccess, T? data, string message, List<Error>? errors)
  {
    IsSuccess = isSuccess;
    Data = data;
    Message = message;
    Errors = errors ?? new List<Error>();
  }

  public bool IsSuccess { get; set; }
  public T? Data { get; set; } = default;
  public string Message { get; set; }
  public List<Error>? Errors { get; set; } = default;

  public static ApiResponse<T> SuccessResponse(T data, string message) 
    => new ApiResponse<T> (true, data, message, new List<Error>());

  public static ApiResponse<T> FailureResponse(List<Error> errors, string message) 
    => new ApiResponse<T> (false, default, message, errors);
}
