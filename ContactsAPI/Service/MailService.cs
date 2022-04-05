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

        public async Task SendEmail2(MailRequest mailRequest)
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

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
             var email = new MimeMessage();
             email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
             email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
             email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
             email.Subject = mailRequest.Subject;
             var builder = new BodyBuilder();
             if (mailRequest.Attachments != null)
             {
                 byte[] fileBytes;
                 foreach (var file in mailRequest.Attachments)
                 {
                     if (file.Length > 0)
                     {
                         using (var ms = new MemoryStream())
                         {
                             file.CopyTo(ms);
                             fileBytes = ms.ToArray();
                         }
                         builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                     }
                 }
             }

             builder.HtmlBody = mailRequest.Body;
             email.Body = builder.ToMessageBody();
             using var smtp = new MailKit.Net.Smtp.SmtpClient();
             smtp.Connect(_mailSettings.Host, _mailSettings.Port, true);
             smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
             await smtp.SendAsync(email);
             smtp.Disconnect(true);

        }
    }
}
