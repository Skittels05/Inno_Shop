using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Interfaces.Repositories;
using System;
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
            return base.FindAll(trackChanges).Where(p => !p.IsDeleted);
        }

        public override IQueryable<Product> FindByCondition(Expression<Func<Product, bool>> expression, bool trackChanges = false)
        {
            return base.FindByCondition(expression, trackChanges).Where(p => !p.IsDeleted);
        }

        public async Task DeleteAsync(Product product)
        {
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task SoftDeleteByUserAsync(Guid userId)
        {
            var products = await _context.Products
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .ToListAsync();

            if (!products.Any())
                return;

            products.ForEach(p =>
            {
                p.IsDeleted = true;
                p.DeletedAt = DateTime.UtcNow;
            });

            _context.Products.UpdateRange(products);
            await _context.SaveChangesAsync();
        }

        public async Task RestoreByUserAsync(Guid userId)
        {
            var products = await _context.Products
                .Where(p => p.UserId == userId && p.IsDeleted)
                .ToListAsync();

            if (!products.Any())
                return;

            products.ForEach(p =>
            {
                p.IsDeleted = false;
                p.DeletedAt = null;
            });

            _context.Products.UpdateRange(products);
            await _context.SaveChangesAsync();
        }

    }

}
