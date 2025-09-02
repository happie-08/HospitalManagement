using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

public class SendGridEmailSender : IEmailSender
{
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SendGridEmailSender(IOptions<SendGridSettings> options)
    {
        _apiKey = options.Value.ApiKey;
        _fromEmail = options.Value.FromEmail;
        _fromName = options.Value.FromName;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, _fromName);
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
        await client.SendEmailAsync(msg);
    }
}

public class SendGridSettings
{
    public string ApiKey { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
}
