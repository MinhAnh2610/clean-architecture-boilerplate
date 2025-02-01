namespace CleanArchitecture.Application.Common;

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

  public static Result<T> Success(T data, int status) => new Result<T>(true, new List<Error>(), data, status);

  public static Result<T> Failure(List<Error> errors, int status) => new Result<T>(false, errors, default, status);
}
