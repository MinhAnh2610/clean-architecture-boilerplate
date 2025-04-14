using FluentValidation.Results;

namespace CleanArchitecture.Application.Extensions
{
  public interface IErrorFactory
  {
    (Error err, int statusCode) CreateNotFoundError(string objectName);
    (List<Error> errs, int statusCode) CreateValidationError(string objectName, ValidationResult validationResult);
    (Error err, int statusCode) CreateAlreadyExistsError(string objectName);
    (Error err, int statusCode) CreateDatabaseError(string objectName);
    (Error err, int statusCode) CreateFileCreatedFailed(string objectName);
    (Error err, int statusCode) CreateInvalidDates();
    (Error err, int statusCode) CreateInvalidFileError();
    (Error err, int statusCode) CreateInvalidCouponError();
  }
}