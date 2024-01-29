using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project_backend.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project_backend.Controllers
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDBContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly string _imagePath;

        public ProductsController(ProductDBContext context, ILogger<ProductsController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _imagePath = Path.Combine(configuration.GetValue<string>("StaticFilesPath"), "images");
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                _logger.LogInformation($"Product with ID {id} not found.");
                return NotFound();
            }

            return product;
        }

        [Authorize]
        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] string name, [FromForm] decimal price, [FromForm] string description, IFormFile imageFile)
        {
            var product = new Product { Name = name, Price = price, Description = description }; // Include Description property

            if (imageFile != null && !ValidateImageFile(imageFile))
            {
                return BadRequest("Invalid image file.");
            }

            try
            {
                if (imageFile != null)
                {
                    // SaveImageAsync now returns the correctly formatted relative path
                    product.ImagePath = await SaveImageAsync(imageFile);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetProduct", new { id = product.ID }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product.");
                return StatusCode(500, "An error occurred while creating the product.");
            }
        }




        [Authorize]
        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromForm] Product product, IFormFile imageFile)
        {
            if (id != product.ID)
            {
                return BadRequest();
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            if (imageFile != null && !ValidateImageFile(imageFile))
            {
                return BadRequest("Invalid image file.");
            }

            try
            {
                if (imageFile != null)
                {
                    if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                    {
                        DeleteImage(existingProduct.ImagePath);
                    }

                    string fileName = await SaveImageAsync(imageFile);
                    product.ImagePath = Path.Combine(_imagePath, fileName);
                }

                _context.Entry(existingProduct).CurrentValues.SetValues(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating product with ID {id}.");
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }

        [Authorize]
        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    DeleteImage(product.ImagePath);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting product with ID {id}.");
                return StatusCode(500, "An error occurred while deleting the product.");
            }
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), _imagePath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Normalize the path to use forward slashes
            return Path.Combine("/static/images", fileName).Replace("\\", "/");
        }


        private void DeleteImage(string imagePath)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        private bool ValidateImageFile(IFormFile file)
        {
            // Add image validation logic (file type, size, etc.)
            return true;
        }
    }
}
