namespace CleanArchitecture.Application.Common;

public class ApiResponse<T>
{
  public bool Success { get; set; }
  public T Data { get; set; }
  public string Message { get; set; }
  public List<string> Errors { get; set; }

  public ApiResponse(T data, string message = null)
  {
    Success = true;
    Data = data;
    Message = message;
    Errors = null;
  }

  public ApiResponse(string message, List<string> errors)
  {
    Success = false;
    Data = default;
    Message = message;
    Errors = errors;
  }
}
