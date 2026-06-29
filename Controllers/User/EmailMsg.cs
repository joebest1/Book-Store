using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using (var client = new SmtpClient(_settings.SmtpServer, _settings.Port))
        {
            client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            client.EnableSsl = true;

            var mail = new MailMessage()
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);
            await client.SendMailAsync(mail);
        }
    }
}
