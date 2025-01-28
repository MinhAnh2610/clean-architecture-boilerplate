namespace CleanArchitecture.Application.Common;

public class ApiResponse<T>
{
  public ApiResponse(bool isSuccess, T data = default, string message = null, List<string> errors = null)
  {
    IsSuccess = isSuccess;
    Data = data;
    Message = message;
    Errors = errors ?? new List<string>();
  }

  public bool IsSuccess { get; set; }
  public T Data { get; set; }
  public string Message { get; set; }
  public List<string> Errors { get; set; }
}
