namespace CleanArchitecture.Application.ServiceContracts;

public interface IEmailService
{
  Task<Result<string>> SendEmailAsync(string to, string subject, string body);
}
