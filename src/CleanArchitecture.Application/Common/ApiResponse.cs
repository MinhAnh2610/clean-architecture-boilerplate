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
}
