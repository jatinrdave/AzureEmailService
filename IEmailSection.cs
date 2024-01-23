using System;
using System.Collections.Generic;
using System.Text;

namespace Windowmaker.Data.Models.Interfaces
{
    public interface IEmailSection
    {
        /// <summary>
        /// SendGrid API key
        /// </summary>
        public string SendGridAPIKey { get; set; }

        /// <summary>
        /// Email address of sender which is set as the source(from) of email.
        /// </summary>
        public string SenderEmail { get; set; }
        string ConnectionString { get; set; }
    }
}
