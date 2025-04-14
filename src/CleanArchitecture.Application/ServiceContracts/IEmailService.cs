using CleanArchitecture.Application.DTOs.Email;

namespace CleanArchitecture.Application.ServiceContracts;

public interface IEmailService
{
  Task SendEmailAsync(EmailMessage message);
}
