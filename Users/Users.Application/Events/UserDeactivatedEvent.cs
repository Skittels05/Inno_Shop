using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.Events
{
    public record UserDeactivatedEvent(Guid UserId, DateTime DeactivatedAt);
}
