using CleanArchitecture.Application.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Application.Exceptions
{
  public class ErrorFactory : IErrorFactory
  {
    public (Error err, int statusCode) CreateNotFoundError(string objectName)
    {
      return (new Error($"{objectName}.NotFound", $"{objectName} not found."), StatusCodes.Status404NotFound);
    }

    public (List<Error> errs, int statusCode) CreateValidationError(string objectName,
      ValidationResult validationResult)
    {
      var errs = new List<Error>();
      foreach (var err in validationResult.Errors)
      {
        errs.Add(new Error($"{objectName}.Invalid", err.ErrorMessage));
      }

      return (errs, StatusCodes.Status400BadRequest);
    }

    // public (, int statusCode) CreateValidationError(string objectName)
    // {
    //   return (new Error($"{objectName}.Invalid", $"{objectName} is not valid."), StatusCodes.Status400BadRequest);
    // }

    public (Error err, int statusCode) CreateAlreadyExistsError(string objectName)
    {
      return (new Error($"{objectName}.Duplicate", $"{objectName} already exists."), StatusCodes.Status409Conflict);
    }

    public (Error err, int statusCode) CreateDatabaseError(string objectName)
    {
      return new(new Error($"{objectName}.DatabaseFailed", $"Database operation with {objectName} failed."), StatusCodes.Status500InternalServerError);
    }

    public (Error err, int statusCode) CreateFileCreatedFailed(string objectName)
    {
      return (new Error($"{objectName}.FileCreatedFailed", $"{objectName} file creating operation failed."), StatusCodes.Status500InternalServerError);
    }

    public (Error err, int statusCode) CreateInvalidDates()
    {
      return (new Error("Input.InvalidDates", "The dates inputed are invalid."), StatusCodes.Status400BadRequest);
    }

    public (Error err, int statusCode) CreateInvalidFileError()
    {
      return new(new Error("Input.InvalidFile", "The file input is invalid."), StatusCodes.Status400BadRequest);
    }

    public (Error err, int statusCode) CreateInvalidCouponError()
    {
      return new(new Error("Coupon.InvalidCoupon", "The coupon code is invalid."), StatusCodes.Status400BadRequest);
    }
  }
}