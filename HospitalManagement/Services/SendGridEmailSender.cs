using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridOptions _options;

        public SendGridEmailSender(IOptions<SendGridOptions> optionsAccessor)
        {
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(_options.SendGridKey))
                throw new ArgumentNullException(nameof(_options.SendGridKey), "SendGrid API Key is missing!");

            var client = new SendGridClient(_options.SendGridKey);

            var from = new EmailAddress(_options.SenderEmail, _options.FromName ?? "Hospital Management");
            var to = new EmailAddress(email);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: "Please confirm your account.",
                                                   htmlContent: message);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"SendGrid failed: {response.StatusCode}");
            }
        }
    }
    public class SendGridOptions
    {
        public string SendGridKey { get; set; }
        public string SenderEmail { get; set; }
        public string FromName { get; set; }
    }

}
