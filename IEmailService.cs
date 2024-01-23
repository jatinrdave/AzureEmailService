using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
        /// <summary>
        /// Provides email service.
        /// </summary>
        public interface IEmailService 
        {
            /// <summary>
            /// Send email to single recepient.
            /// </summary>
            /// <param name="from">Sender email</param>
            /// <param name="subject">Subject line</param>
            /// <param name="to">Recepient email</param>
            /// <param name="bccs">List of BCC recepients</param>
            /// <param name="message">Message body</param>
            /// <param name="downloadFilename">Attachment name</param>
            /// <param name="data">Attachment file</param>
            /// <returns>Awaitable task</returns>
            Task SendEmail(string from, string subject, string to, string message, IList<string> bccs = null, string downloadFilename = null, byte[] data = null);

            /// <summary>
            /// Send email to multiple recepient.
            /// </summary>
            /// <param name="from">Sender email</param>
            /// <param name="subject">Subject line</param>
            /// <param name="tos">List of recepients</param>
            /// <param name="bccs">List of BCC recepients</param>
            /// <param name="message">Message body</param>
            /// <param name="downloadFilename">Attachment name</param>
            /// <param name="data">Attachment file</param>
            /// <returns>Awaitable task</returns>
            Task SendEmailToMultipleRecepients(string from, string subject, IList<string> tos, string message, IList<string> bccs = null, string downloadFilename = null, byte[] data = null);
        //Task SendEmailToMultipleRecepients(string sFrom, string sSubject, string[] sTo, string resultString, object value, object attachmentFileName, object attachmentFileData);
    }
    }

