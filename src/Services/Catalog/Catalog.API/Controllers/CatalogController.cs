using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Entities;
using Catalog.API.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepo _productRepo;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepo productRepo,
            ILogger<CatalogController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            var products = await _productRepo.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<ActionResult<Product>> GetById(string id)
        {
            var product = await _productRepo.GetProduct(id);
            if (product == null)
            {
                _logger.LogWarning("Product with id: {Id}, not found", id);
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("[action]/{category}", Name = "GetProductByCategory")]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products = await _productRepo.GetProductByCategory(category);
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _productRepo.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new {id = product.Id}, product);
        }
        
        [HttpPut]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            var res = await _productRepo.UpdateProduct(product);
            return Ok(res);
        }
        
        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var res = await _productRepo.DeleteProduct(id);
            return Ok(res);
        }
    }
}