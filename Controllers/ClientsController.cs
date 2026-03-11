using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaS.Api.Data;
using SaaS.Api.Models;

namespace SaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Crear Clientse
        [HttpPost]
        public async Task<IActionResult> Create(Clients Clients)
        {
            _context.Clients.Add(Clients);
            await _context.SaveChangesAsync();
            return Ok(Clients);
        }

        // Obtener todos los Clientses del tenant
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var Clients = await _context.Clients.ToListAsync(); // filtrado por CompanyId automáticamente
            return Ok(Clients);
        }

        // Obtener Clientse por id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var Clients = await _context.Clients.FindAsync(id);
            if (Clients == null)
                return NotFound("Clients not found.");
            return Ok(Clients);
        }

        // Actualizar Clientse
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Clients updatedClients)
        {
            var Clients = await _context.Clients.FindAsync(id);
            if (Clients == null)
                return NotFound("Clients not found.");

            Clients.Name = updatedClients.Name;
            Clients.Email = updatedClients.Email;
            Clients.Phone = updatedClients.Phone;
            Clients.IsActive = updatedClients.IsActive;

            await _context.SaveChangesAsync();
            return Ok(Clients);
        }

        // Eliminar Clientse
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var Clients = await _context.Clients.FindAsync(id);
            if (Clients == null)
                return NotFound("Clients not found.");

            _context.Clients.Remove(Clients);
            await _context.SaveChangesAsync();
            return Ok("Clients deleted successfully.");
        }
    }
}