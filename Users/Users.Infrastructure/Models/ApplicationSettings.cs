using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Infrastructure.Models
{
    public class ApplicationSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ConfirmEmailPath { get; set; } = string.Empty;
        public string ResetPasswordPath { get; set; } = string.Empty;
    }
}
