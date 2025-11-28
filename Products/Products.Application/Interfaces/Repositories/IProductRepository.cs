using Products.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Products.Domain.Interfaces.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> FindAll(bool trackChanges = false);
        IQueryable<Product> FindByCondition(Expression<Func<Product, bool>> expression, bool trackChanges = false);
        Task<Product> GetByIdAsync(Guid productId, Guid userId);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }

}
