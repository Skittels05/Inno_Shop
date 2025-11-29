using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Infrastructure.Exceptions
{
    public class EmailConfigurationException : EmailException
    {
        public EmailConfigurationException(string message) : base(message)
        {
        }

        public EmailConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
