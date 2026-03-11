using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaS.Api.Common;
using SaaS.Api.Data;
using SaaS.Api.DTOs.Products;
using SaaS.Api.Models;

namespace SaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Crear producto
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            var product = new Products
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock,
               // CompanyId = HttpContext.GetCompanyId() // Asignar el tenant actual
            };
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // Obtener todos los productos del tenant (empresa)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products.ToListAsync(); // Filtro automático por CompanyId gracias al QueryFilter
            return Ok(products);
        }

        // Obtener producto por id (solo del tenant actual)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            return Ok(product);
        }

        // Actualizar producto
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound("Product not found.");

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // Eliminar producto
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("product not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok("product deleted successfully.");
        }
    }
}