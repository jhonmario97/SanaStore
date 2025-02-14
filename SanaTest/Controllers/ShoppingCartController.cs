using Microsoft.AspNetCore.Mvc;
using SanaTest.Models;
using SanaTest.Services;

namespace SanaTest.Controllers
{
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            var cartItems = await _shoppingCartService.GetCartItemsAsync();
            return Ok(cartItems);
        }
        [HttpPost("add")]
        public async Task<ActionResult> AddToCart(int productId, int quantity)
        {
            await _shoppingCartService.AddToCartAsync(productId, quantity);
            return Ok();
        }

        [HttpPost("save")]
        public async Task<ActionResult> ProcessOrder(int customerId)
        {
            await _shoppingCartService.ProcessOrderAsync(customerId);
            return Ok();
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> RemoveCartItem(int productId)
        {
            await _shoppingCartService.RemoveCartItemAsync(productId);
            return Ok();
        }

    }
}
