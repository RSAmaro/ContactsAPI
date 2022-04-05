using ContactsAPI.Models;

namespace ContactsAPI.Service
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);

        Task SendEmail2(MailRequest mailRequest);
    }
}
