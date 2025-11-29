using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Events
{
    public record UserActivatedEvent(Guid UserId, DateTime ActivatedAt);
}
