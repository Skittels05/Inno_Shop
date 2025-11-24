using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Enums;

namespace Users.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive {  get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string? PasswordRecoveryToken { get; set; }
        public DateTime? PasswordRecoveryTokenExpiration { get; set; }
    }
}
