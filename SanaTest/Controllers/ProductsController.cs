using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanaTest.Models;
using SanaTest.Services;

namespace SanaTest.Controllers
{



    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        //{
        //    var products = await _productService.GetProductsAsync();
        //    return Ok(products);
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int pageNumber = 1, int pageSize = 10)
        {
            var products = await _productService.GetProductsAsync(pageNumber, pageSize);
            return Ok(products);
        }


    }
}
