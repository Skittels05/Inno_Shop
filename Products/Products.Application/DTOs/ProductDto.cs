using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.DTOs
{
    public record ProductDto(Guid Id, string Name, string Description, bool IsAvailable);
}
