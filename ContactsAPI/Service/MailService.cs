using ContactsAPI.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Net;
using System.Net.Mail;

namespace ContactsAPI.Service
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            System.Net.Mail.SmtpClient smtp = new();
            MailMessage mailMessage = new();
            mailMessage = new()
            {
                From = new MailAddress(_mailSettings.Mail),
                Subject = mailRequest.Subject,
                Body = mailRequest.Body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(mailRequest.ToEmail);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Port = _mailSettings.Port;
            smtp.Credentials = new NetworkCredential
            {
                UserName = _mailSettings.Mail,
                Password = _mailSettings.Password
            };

            await smtp.SendMailAsync(mailMessage);

            mailMessage.Dispose();
            mailMessage = null;
            smtp.Dispose();
            smtp = null;
        }
    }
}
