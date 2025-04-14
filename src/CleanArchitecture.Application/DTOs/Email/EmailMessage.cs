using MimeKit;

namespace CleanArchitecture.Application.DTOs.Email;

public class EmailMessage
{
  public List<MailboxAddress> To { get; set; } = [];
  public string Subject { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public EmailMessage(IEnumerable<string> to, string subject, string content)
  {
    To = [.. to.Select(x => new MailboxAddress("email", x))];
    Subject = subject;
    Content = content;
  }
}
