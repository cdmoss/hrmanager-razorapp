using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MHFoodBank.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string recipientEmail, string subject, string message);
    }

    public class MailKitEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public MailKitEmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string message)
        {
            // Smtp client
            var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);

            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));
            mail.To.Add(MailboxAddress.Parse(recipientEmail));
            mail.Subject = subject;
            mail.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            client.Send(mail);
            await client.DisconnectAsync(true);
        }
    }
}
