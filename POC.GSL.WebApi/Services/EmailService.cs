using Microsoft.Extensions.Options;
using POC.GSL.WebApi.Model;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace POC.GSL.WebApi.Services
{
    public class EmailService
    {
        public EmailService(IOptions<MessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public MessageSenderOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute("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", subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("umberto.mif@gmail.com", "POC"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.TrackingSettings = new TrackingSettings
            {
                ClickTracking = new ClickTracking { Enable = false }
            };

            return client.SendEmailAsync(msg);
        }


    }
}
