using Microsoft.AspNetCore.Mvc;
using SaaS.Api.Data;
using SaaS.Api.DTOs;
using SaaS.Api.Models;

namespace SaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            // Verificar si el email ya existe
            if (_context.Users.Any(u => u.Email == request.Email))
                return BadRequest("Email already exists.");

            // Crear empresa
            var company = new Company
            {
                Name = request.CompanyName
            };

            // Crear usuario admin
            var user = new User
            {
                Name = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Admin",
                Company = company
            };

            _context.Companies.Add(company);
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok("Company and admin user created successfully.");
        }
    }
}