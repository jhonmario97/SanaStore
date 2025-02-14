using Microsoft.EntityFrameworkCore;
using SanaTest.Models;

namespace SanaTest.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<Product>> GetProductByIdAsync(int ProductId);

        Task<bool> ValidateStockAsync(int productId, int quantity);

        Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize);

    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByIdAsync(int ProductId)
        {
            return await _context.Products
                .Where(p => p.ProductId == ProductId)
                .ToListAsync();
        }

        public async Task<bool> ValidateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            return product != null && product.Stock >= quantity;
        }
    }
}

