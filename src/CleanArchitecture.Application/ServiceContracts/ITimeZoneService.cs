namespace CleanArchitecture.Application.ServiceContracts;

public interface ITimeZoneService
{
  DateTime ConvertToLocalTime(DateTime utcTime);
  DateTime ConvertToUtcTime(DateTime localTime);
}
