using ecom.product.application.ProductApp;
using ecom.product.domain.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ecom.ProductService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductApplication _productApplication;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductApplication productApplication, ILogger<ProductController> logger)
        {
            _productApplication = productApplication;
            _logger = logger;
        }

        [HttpGet(Name = "GetProducts")]
        [AllowAnonymous] // Allow public access to product list
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            _logger.LogInformation("Getting all products");
            return Ok(await _productApplication.ListAsync());
        }

        [HttpGet("{id}", Name = "GetById")]
        [AllowAnonymous] // Allow public access to product details
        public async Task<ActionResult<Product>> GetById(string id)
        {
            _logger.LogInformation($"Getting product by id: {id}");
            var product = await _productApplication.GetAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost (Name = "product")]
        [Authorize(Roles = "Admin")] // Only admins can add products
        public async Task<ActionResult<string>> Add(Product product)
        {
            _logger.LogInformation($"Adding new product: {product.Name}");
            try
            {
                var id = await _productApplication.AddAsync(product);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("product/{id}/updatequantity/{quantity}")]
        [Authorize(Roles = "Admin")] // Only admins can update product quantity
        public async Task<ActionResult<int>> UpdateProductQuantity(string id, int quantity)
        {
            _logger.LogInformation($"Updating product quantity: {id}, quantity: {quantity}");
            try
            {
                var result = await _productApplication.UpdateQuantityAsync(id, quantity);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating product quantity: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("generateproductdescription")]
        [Authorize(Roles = "Admin")] // Only admins can generate product descriptions
        public async Task<ActionResult<Product>> GenerateProductDescription([FromBody] Product productDetails)
        {
            _logger.LogInformation($"Generating product description for: {productDetails.Name}");
            try
            {
                var result = await _productApplication.GenerateProductDescription(productDetails);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating product description: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}