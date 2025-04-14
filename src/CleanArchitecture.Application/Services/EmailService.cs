using CleanArchitecture.Application.DTOs.Email;
using CleanArchitecture.Application.ServiceContracts;
using MailKit.Net.Smtp;
using MimeKit;

namespace CleanArchitecture.Application.Services;

public class EmailService : IEmailService
{
  private readonly EmailConfiguration _emailConfig;
  public EmailService(EmailConfiguration emailConfig)
  {
    _emailConfig = emailConfig;
  }
  public async Task SendEmailAsync(EmailMessage message)
  {
    var emailMessage = CreateEmailMessage(message);
    await SendAsync(emailMessage);
  }

  private async Task SendAsync(MimeMessage emailMessage)
  {
    using var client = new SmtpClient();
    try
    {
      await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
      client.AuthenticationMechanisms.Remove("XOAUTH2");
      await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

      await client.SendAsync(emailMessage);
    }
    catch (Exception)
    {
      throw;
    }
    finally
    {
      await client.DisconnectAsync(true);
      client.Dispose();
    }
  }

  private MimeMessage CreateEmailMessage(EmailMessage message)
  {
    var emailMessage = new MimeMessage();
    emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
    emailMessage.To.AddRange(message.To);
    emailMessage.Subject = message.Subject;

    var bodyBuilder = new BodyBuilder()
    {
      HtmlBody = message.Content
    };

    emailMessage.Body = bodyBuilder.ToMessageBody();

    return emailMessage;
  }
}
