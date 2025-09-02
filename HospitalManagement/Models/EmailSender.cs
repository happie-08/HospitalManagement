using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;


public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentNullException(nameof(apiKey), "SendGrid API Key is missing.");

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("happykapupara810@gmail.com", "HospitalManagement");
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
        var response = await client.SendEmailAsync(msg);
    }
}
