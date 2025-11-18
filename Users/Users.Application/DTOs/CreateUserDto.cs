using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Enums;

namespace Users.Application.DTOs
{
    public record CreateUserDto(
        string Name,
        string Email,
        string Password,
        Role Role
    );
}
