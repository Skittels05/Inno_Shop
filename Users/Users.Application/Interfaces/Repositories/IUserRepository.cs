using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        IQueryable<User> FindAll(bool trackChanges = false);
        IQueryable<User> FindByCondition(Expression<Func<User, bool>> expression, bool trackChanges = false);
        Task<User> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email, bool trackChanges);
        Task<User?> GetByNameAsync(string name, bool trackChanges = false);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
