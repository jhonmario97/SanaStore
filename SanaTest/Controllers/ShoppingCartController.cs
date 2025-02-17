using Microsoft.AspNetCore.Mvc;
using SanaTest.Models;
using SanaTest.Services;

namespace SanaTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        [HttpPut("update")]
        public async Task<ActionResult> UpdateCart(int productId, int quantity)
        {
            await _shoppingCartService.UpdateCartItemQuantityAsync(productId, quantity);
            return Ok();
        }
        [HttpPost("save")]
        public async Task<ActionResult> ProcessOrder(Customer customer)
        {
            await _shoppingCartService.ProcessOrderAsync(customer);
            return Ok();
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> RemoveCartItem(int productId)
        {
            await _shoppingCartService.RemoveCartItemAsync(productId);
            return Ok(new CartItem { ProductId=productId} );
        }

    }
}
