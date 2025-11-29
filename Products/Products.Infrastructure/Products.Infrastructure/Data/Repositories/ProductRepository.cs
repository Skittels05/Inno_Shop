using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Products.Infrastructure.Data.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ProductContext context) : base(context) { }

        public async Task<Product> GetByIdAsync(Guid productId, Guid userId)
        {
            return await _context.Products
                                 .Where(p => p.Id == productId && p.UserId == userId)
                                 .FirstOrDefaultAsync();
        }

        public override IQueryable<Product> FindAll(bool trackChanges = false)
        {
            var query = base.FindAll(trackChanges).Where(p => !p.IsDeleted);
            return trackChanges ? query : query.AsNoTracking();
        }

        public override IQueryable<Product> FindByCondition(Expression<Func<Product, bool>> expression, bool trackChanges = false)
        {
            var query = base.FindByCondition(expression, trackChanges).Where(p => !p.IsDeleted);
            return trackChanges ? query : query.AsNoTracking();
        }

        public async Task<IEnumerable<Product>> GetProductsByUserAsync(Guid userId, bool includeDeleted = false)
        {
            var query = _context.Products.Where(p => p.UserId == userId);

            if (!includeDeleted)
                query = query.Where(p => !p.IsDeleted);

            return await query.ToListAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Product> products)
        {
            _context.Products.UpdateRange(products);
            await _context.SaveChangesAsync();
        }
    }
}