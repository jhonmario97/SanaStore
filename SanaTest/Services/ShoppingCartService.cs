using SanaTest.Models;

namespace SanaTest.Services
{

    public interface IShoppingCartService
    {
        Task AddToCartAsync(int productId, int quantity);
        Task<IEnumerable<CartItem>> GetCartItemsAsync();
        Task UpdateCartItemQuantityAsync(int productId, int quantity);
        Task RemoveCartItemAsync(int productId);
        Task ProcessOrderAsync(int customerId);
    }
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly AppDbContext _context;
        private readonly List<CartItem> _cartItems = new List<CartItem>();

        public ShoppingCartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || !await ValidateStockAsync(productId, quantity))
            {
                throw new InvalidOperationException("Product not found or insufficient stock.");
            }
            //check if item exists on cartItems if not create item
            var existingItem = _cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Title,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
        }

        public Task<IEnumerable<CartItem>> GetCartItemsAsync()
        {
            return Task.FromResult(_cartItems.AsEnumerable());
        }

        public Task UpdateCartItemQuantityAsync(int productId, int quantity)
        {
            var item = _cartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
            return Task.CompletedTask;
        }

        public Task RemoveCartItemAsync(int productId)
        {
            var item = _cartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _cartItems.Remove(item);
            }
            return Task.CompletedTask;
        }

        private async Task<bool> ValidateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            return product != null && product.Stock >= quantity;
        }

        public async Task ProcessOrderAsync(int customerId)
        {
            //set order information 
            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = _cartItems.Sum(item => item.Price * item.Quantity)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            //set order detail information  from  cartItems
            foreach (var item in _cartItems)
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
            _cartItems.Clear();
        }

    
    }
}
