using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Infrastructure.Exceptions
{
    public class EmailSendingException : EmailException
    {
        public string Email { get; }
        public string Subject { get; }

        public EmailSendingException(string email, string subject, string message, Exception innerException = null)
            : base(message, innerException)
        {
            Email = email;
            Subject = subject;
        }
    }
}
