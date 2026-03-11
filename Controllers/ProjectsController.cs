using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaS.Api.Data;
using SaaS.Api.Models;
using SaaS.Api.Common;

namespace SaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Crear proyecto
        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            // No necesitamos recibir CompanyId en body, se asigna automáticamente por el DbContext multi-tenant (Ojo)
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return Ok(project);
        }

        // Obtener todos los proyectos del tenant (empresa)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var projects = await _context.Projects.ToListAsync(); // Filtro automático por CompanyId gracias al QueryFilter
            return Ok(projects);
        }

        // Obtener proyecto por id (solo del tenant actual)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound("Project not found.");

            return Ok(project);
        }

        // Actualizar proyecto
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Project updatedProject)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound("Project not found.");

            project.Name = updatedProject.Name;
            

            await _context.SaveChangesAsync();
            return Ok(project);
        }

        // Eliminar proyecto
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound("Project not found.");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Ok("Project deleted successfully.");
        }
    }
}