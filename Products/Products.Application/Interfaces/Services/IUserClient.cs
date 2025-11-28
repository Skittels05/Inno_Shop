using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Interfaces.Services
{
    public interface IUserClient
    {
        Task<bool> IsUserActiveAsync(Guid userId, CancellationToken ct = default);
    }

}
