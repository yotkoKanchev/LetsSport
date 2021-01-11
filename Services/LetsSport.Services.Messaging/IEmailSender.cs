namespace LetsSport.Services.Messaging
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Common;

    public interface IEmailSender
    {
        Task SendEmailAsync(
            string to,
            string subject,
            string htmlContent,
            string fromName = GlobalConstants.SystemName,
            string from = GlobalConstants.SystemEmail,
            IEnumerable<EmailAttachment> attachments = null);
    }
}
