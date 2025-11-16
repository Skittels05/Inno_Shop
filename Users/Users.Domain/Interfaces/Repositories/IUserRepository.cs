using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        IQueryable<User> FindAll(bool trackChanges = false);
        IQueryable<User> FindByCondition(Expression<Func<User, bool>> expression, bool trackChanges = false);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
