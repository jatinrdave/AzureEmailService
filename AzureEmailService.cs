using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;
//using Microsoft.Reporting.Map.WebForms.BingMaps;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Windowmaker.Data.Models.Interfaces;
using Windowmaker.Data.Services.Interfaces;

namespace WindowmakerWindowsFormTestClient
{

    /// <summary>
    /// Provide email services by Azure Communication Services.
    /// </summary>
    public class AzureEmailService : IEmailService
    {
        private ILogger<AzureEmailService> _logger;
        private readonly IEmailSection _emailSection;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AzureEmailService(IEmailSection emailSection,
            ILogger<AzureEmailService> logger)
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

            var connectionString = _emailSection.ConnectionString;
            var emailClient = new EmailClient(connectionString);

            var sender = from;// "<SENDER_EMAIL>";
            var recipient = to;//"<RECIPIENT_EMAIL>";
            //var subject = "Send email plain text sample";

            var emailContent = new EmailContent(subject)
            {
                Html = message
                //PlainText = "This is plain text mail send test body \n Best Wishes!!",
            };

            var emailMessage = new EmailMessage(sender, recipient, emailContent);

            if (downloadFilename != null && data != null)
            {
                // Add pdf attachment
                byte[] pdfBytes = data;// File.ReadAllBytes("attachment.pdf");
                var pdfContentBinaryData = new BinaryData(pdfBytes);
                var pdfEmailAttachment = new EmailAttachment(downloadFilename, MediaTypeNames.Application.Pdf, pdfContentBinaryData);
                emailMessage.Attachments.Add(pdfEmailAttachment);
            }

            try
            {
                var emailSendOperation = await emailClient.SendAsync(
                    wait: WaitUntil.Completed,
                    message: emailMessage);

                _logger.LogInformation($"Email Sent. Status = {emailSendOperation.Value.Status}");

                /// Get the OperationId so that it can be used for tracking the message for troubleshooting
                string operationId = emailSendOperation.Id;
                _logger.LogInformation($"Email operation id = {operationId}");
            }
            catch (RequestFailedException ex)
            {
                /// OperationID is contained in the exception message and can be used for troubleshooting purposes
                _logger.LogError($"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");
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
            var connectionString = _emailSection.ConnectionString;
            var emailClient = new EmailClient(connectionString);

            var sender = from;// "<SENDER_EMAIL>";
            //var recipient = tos;//"<RECIPIENT_EMAIL>";
            //var subject = "Send email plain text sample";

            var emailContent = new EmailContent(subject)
            {
                Html = message
            };

            // Create the To list
            var toRecipients = new List<EmailAddress>();
            foreach (var x in tos)
            {
                toRecipients.Add(new EmailAddress(
                     address: x,
                     displayName: x));
            };
            // Create the BCC list
            var bccRecipients = new List<EmailAddress>();
            foreach (var x in bccs)
            {
                bccRecipients.Add(new EmailAddress(
                     address: x,
                     displayName: x));
            };


            var emailRecipients = new EmailRecipients(toRecipients, null, bccRecipients);
            var emailMessage = new EmailMessage(sender, emailRecipients, emailContent);

            if (downloadFilename != null && data != null)
            {
                // Add pdf attachment
                byte[] pdfBytes = data;// File.ReadAllBytes("attachment.pdf");
                var pdfContentBinaryData = new BinaryData(pdfBytes);
                var pdfEmailAttachment = new EmailAttachment(downloadFilename, MediaTypeNames.Application.Pdf, pdfContentBinaryData);
                emailMessage.Attachments.Add(pdfEmailAttachment);
            }

            try
            {
                var emailSendOperation = await emailClient.SendAsync(
                    wait: WaitUntil.Completed,
                    message: emailMessage);

                _logger.LogInformation($"Email Sent. Status = {emailSendOperation.Value.Status}");

                /// Get the OperationId so that it can be used for tracking the message for troubleshooting
                string operationId = emailSendOperation.Id;
                _logger.LogInformation($"Email operation id = {operationId}");
            }
            catch (RequestFailedException ex)
            {
                /// OperationID is contained in the exception message and can be used for troubleshooting purposes
                _logger.LogError($"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");

            }
        }
    }
}
