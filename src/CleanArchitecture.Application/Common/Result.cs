namespace CleanArchitecture.Application.Common;

public class Result<T>
{
  private Result(bool isSuccess, List<Error> errors, T? data)
  {
    if (isSuccess && errors.Count > 0 ||
        !isSuccess && errors.Count == 0)
    {
      throw new ArgumentException("Invalid error", nameof(errors));
    }

    IsSuccess = isSuccess;
    Errors = errors;
    Data = data;
  }

  public bool IsSuccess { get; }

  public bool IsFailure => !IsSuccess;

  public List<Error> Errors { get; }

  public T? Data { get; }

  public static Result<T> Success(T data) => new Result<T>(true, new List<Error>(), data);

  public static Result<T> Failure(List<Error> errors) => new Result<T>(false, errors, default);
}
