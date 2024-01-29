using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace Project_backend.Controllers
{
    [Route("api/carts")]
    [ApiController]
    //[Authorize] // Ensure that only authorized users can access these endpoints
    public class CartController : ControllerBase
    {
        private readonly ProductDBContext _context;

        public CartController(ProductDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the cart for a specific user.
        /// </summary>
        /// <param name="userId">The user ID to retrieve the cart for.</param>
        /// <returns>The cart details.</returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<Cart>> GetCartByUserId(string userId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                                           .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound(new { message = "Cart not found." });
            }

            return Ok(cart);
        }

        /// <summary>
        /// Adds a product to a user's cart.
        /// </summary>
        /// <param name="addProductDto">DTO containing user ID and product ID.</param>
        /// <returns>Status of the operation.</returns>
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProductToCart([FromBody] AddProductDto addProductDto)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                                           .FirstOrDefaultAsync(c => c.UserId == addProductDto.UserId);

            if (cart == null)
            {
                return NotFound(new { message = "Cart not found." });
            }

            var product = await _context.Products.FindAsync(addProductDto.ProductId);
            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            if (cart.Items.Any(p => p.ID == addProductDto.ProductId))
            {
                return BadRequest(new { message = "Product already in the cart." });
            }

            cart.Items.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product added to cart successfully." });
        }

        /// <summary>
        /// Removes a product from a user's cart.
        /// </summary>
        /// <param name="removeProductDto">DTO containing user ID and product ID.</param>
        /// <returns>Status of the operation.</returns>
        [HttpPost("remove-product")]
        public async Task<IActionResult> RemoveProductFromCart([FromBody] RemoveProductDto removeProductDto)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                                           .FirstOrDefaultAsync(c => c.UserId == removeProductDto.UserId);

            if (cart == null)
            {
                return NotFound(new { message = "Cart not found." });
            }

            var product = cart.Items.FirstOrDefault(p => p.ID == removeProductDto.ProductId);
            if (product == null)
            {
                return NotFound(new { message = "Product not found in the cart." });
            }

            cart.Items.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product removed from cart successfully." });
        }

        // DTO classes
        public class AddProductDto
        {
            public string UserId { get; set; }
            public int ProductId { get; set; }
        }

        public class RemoveProductDto
        {
            public string UserId { get; set; }
            public int ProductId { get; set; }
        }
    }
}
