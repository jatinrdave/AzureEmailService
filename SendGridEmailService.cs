using Microsoft.Extensions.Logging;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Windowmaker.Data.Models.BLModels;
using Windowmaker.Data.Models.Interfaces;
using Windowmaker.Data.Services.Interfaces;

namespace Windowmaker.Web.Data.Services.BLServices
{

    /// <summary>
    /// Provide email services by SendGrid.
    /// </summary>
    public class SendGridEmailService : IEmailService
    {
        private ILogger<SendGridEmailService> _logger;
        private IEmailSection _emailSection;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SendGridEmailService(
            ILogger<SendGridEmailService> logger, IEmailSection emailSection) 
        {
            _logger = logger;
            _emailSection = emailSection;
        }

        /// <summary>
        /// Send email to single recepient. It sends the email request to the SendGrid API. Hence, whether the email was actually sent cannot be verified.
        /// </summary>
        /// <param name="from">Sender email</param>
        /// <param name="subject">Subject line</param>
        /// <param name="to">Recepient email</param>
        /// <param name="bccs">List of BCC recepients</param>
        /// <param name="message">Message body</param>
        /// <param name="downloadFilename">Attachment name</param>
        /// <param name="data">Attachment file in Byte[]</param>
        /// <returns>Awaitable task</returns>
        public async Task SendEmail(string from, string subject, string to, string message, IList<string> bccs = null, string downloadFilename = null, byte[] data = null)
        {
            try
            {
                string sendGridAPIKey = _emailSection.SendGridAPIKey;//c.EmailSection.SendGridAPIKey;
                SendGridClient client = new SendGridClient(sendGridAPIKey);

                EmailAddress fromEmail = new EmailAddress(from, "WMWeb");
                EmailAddress toEmail = new EmailAddress(to, "WM User");

                string htmlContent = message;
                SendGridMessage msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, null, htmlContent);

                //Add BCCs to the mail.
                if (bccs != null)
                {
                    foreach (string bcc in bccs)
                        msg.AddBcc(bcc);
                }

                //Add attachment file.
                if (data != null && string.IsNullOrEmpty(downloadFilename))
                {
                    string file = Convert.ToBase64String(data);
                    msg.AddAttachment(downloadFilename, file);
                }
                _logger.LogInformation("Sending email to '{to}' with subject '{subject}'", to, subject);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Send email to multiple recepient. It sends the email request to the SendGrid API. Hence, whether the email was actually sent cannot be verified.
        /// </summary>
        /// <param name="from">Sender email</param>
        /// <param name="subject">Subject line</param>
        /// <param name="tos">List of recepients</param>
        /// <param name="bccs">List of BCC recepients</param>
        /// <param name="message">Message body</param>
        /// <param name="downloadFilename">Attachment name</param>
        /// <param name="data">Attachment file</param>
        /// <returns>Awaitable task</returns>
        public async Task SendEmailToMultipleRecepients(string from, string subject, IList<string> tos, string message, IList<string> bccs = null, string downloadFilename = null, byte[] data = null)
        {
            try
            {
                string sendGridAPIKey = _emailSection.SendGridAPIKey;//_appSettingsService.EmailSection.SendGridAPIKey;
                SendGridClient client = new SendGridClient(sendGridAPIKey);

                EmailAddress fromEmail = new EmailAddress(from);
                List<EmailAddress> toEmails = new List<EmailAddress>();
                foreach (string to in tos)
                {
                    toEmails.Add(new EmailAddress(to));
                }

                string htmlContent = message;
                SendGridMessage msg = MailHelper.CreateSingleEmailToMultipleRecipients(fromEmail, toEmails, subject, null, htmlContent, false);

                if (bccs != null)
                {
                    foreach (string bcc in bccs)
                        msg.AddBcc(bcc);
                }
                if (data != null && string.IsNullOrEmpty(downloadFilename))
                {
                    string file = Convert.ToBase64String(data);
                    msg.AddAttachment(downloadFilename, file);
                }
                _logger.LogInformation("Sending multiple email to '{tos}' with subject '{subject}'", string.Join(",", tos), subject);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
