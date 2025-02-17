using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SanaTest.Models;

namespace SanaTest.Services
{

    public interface IShoppingCartService
    {
        Task AddToCartAsync(int productId, int quantity);

        Task UpdateCartItemQuantityAsync(int productId, int quantity);
        Task RemoveCartItemAsync(int productId);
        Task<Customer> CreateCustomer(Customer customer);
        Task ProcessOrderAsync(Customer customer);
        Task<IEnumerable<CartItem>> GetCartItemsAsync();
    }
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CartCacheKey = "CartItems";

        public ShoppingCartService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task AddToCartAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || !await ValidateStockAsync(productId, quantity))
            {
                throw new InvalidOperationException("Product not found or insufficient stock.");
            }

            // gets cart items from cache
            var cartItems = _cache.Get<List<CartItem>>(CartCacheKey) ?? new List<CartItem>();

            // Check if item exists in cart
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cartItems.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Title,
                    Price = product.Price,
                    Quantity = quantity
                });
            }

            // Save updated  to cache
            _cache.Set(CartCacheKey, cartItems);
        }

        public Task<IEnumerable<CartItem>> GetCartItemsAsync()
        {
            var cartItems = _cache.Get<List<CartItem>>(CartCacheKey) ?? new List<CartItem>();
            return Task.FromResult(cartItems.AsEnumerable());
        }

        public Task UpdateCartItemQuantityAsync(int productId, int quantity)
        {
            var cartItems = _cache.Get<List<CartItem>>(CartCacheKey) ?? new List<CartItem>();
            var item = cartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
            // Save updated  to cache
            _cache.Set(CartCacheKey, cartItems);
            return Task.CompletedTask;
        }

        public Task RemoveCartItemAsync(int productId)
        {
            var cartItems = _cache.Get<List<CartItem>>(CartCacheKey) ?? new List<CartItem>();
            var item = cartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cartItems.Remove(item);
            }
            _cache.Set(CartCacheKey, cartItems);
            return Task.CompletedTask;
        }

        private async Task<bool> ValidateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            return product != null && product.Stock >= quantity;
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            var item =  _context.Customers.Where(c => c.DocumentNumber == customer.DocumentNumber).FirstOrDefault();
            if (item == null)
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            else
            {
                item.FirstName = customer.FirstName;
                item.LastName = customer.LastName;
                item.Email = customer.Email;    
                item.Address = customer.Address;
                item.ZipCode = customer.ZipCode;
                await _context.SaveChangesAsync();
                return item;
            }
           
        }


        public async Task ProcessOrderAsync(Customer customer)
        {

            customer = await CreateCustomer(customer);

           

            var cartItems = _cache.Get<List<CartItem>>(CartCacheKey) ?? new List<CartItem>();

            //set order information 
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cartItems.Sum(item => item.Price * item.Quantity)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            //set order detail information  from  cartItems
            foreach (var item in cartItems)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            cartItems.Clear();
            _cache.Set(CartCacheKey, cartItems);
           
        }

    
    }
}
