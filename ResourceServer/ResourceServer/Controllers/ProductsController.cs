using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceServer.Models;

namespace ResourceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // In-memory list to store products
        private static readonly List<Product> Products = new List<Product>
        {
            new Product { Id = 1, Name = "Product A", Price = 10.0M, Description = "Test Product A" },
            new Product { Id = 2, Name = "Product B", Price = 20.0M, Description = "Test Product B"  },
            new Product { Id = 3, Name = "Product C", Price = 30.0M, Description = "Test Product C"  }
        };
        private static int _nextId = 4; // To auto-increment product IDs
        // GET: api/Products/GetAll
        // No authentication or authorization is required to access this endpoint.
        // Anyone (even unauthenticated users) can call this endpoint.
        [AllowAnonymous]
        [HttpGet("GetAll")]
        public ActionResult<List<Product>> GetAllProduct()
        {
            return Ok(Products);
        }
        // GET: api/Products/GetById/{id}
        // Requires the user to be authenticated but does not enforce any specific role.
        // Any authenticated user can call this endpoint regardless of their role.
        [Authorize]
        [HttpGet("GetById/{id}", Name = "GetProductById")]
        public ActionResult<Product> GetProductById(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            return Ok(product);
        }
        // POST: api/Products/Add
        // Requires the user to be authenticated and to have at least one of the roles in the "UserPolicy".
        // Accessible by Admin, Editor, or User roles.
        [Authorize(Policy = "UserPolicy")]
        [HttpPost("Add")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            product.Id = _nextId++;
            Products.Add(product);
            return CreatedAtRoute("GetProductById", new { id = product.Id }, product);
        }
        // PUT: api/Products/Update/{id}
        // Requires the user to be authenticated and to have at least one of the roles in the "EditorPolicy".
        // Accessible by Admin or Editor roles only.
        [Authorize(Policy = "EditorPolicy")]
        [HttpPut("Update/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingProduct = Products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            return NoContent();
        }
        // DELETE: api/Products/Delete/{id}
        // Requires the user to be authenticated and to have the role defined in the "AdminPolicy".
        // Only Admins can access this endpoint.
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            Products.Remove(product);
            return NoContent();
        }
    }
}
