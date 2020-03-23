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
            string from = GlobalConstants.SystemEmail,
            string fromName = GlobalConstants.SystemName,
            IEnumerable<EmailAttachment> attachments = null);
    }
}
