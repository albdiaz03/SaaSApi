using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SaaS.Api.Data;
using SaaS.Api.DTOs;
using SaaS.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
                return BadRequest("Email already exists.");

            // Verificar que la empresa exista
            var companyExists = _context.Companies
                .Any(c => c.Id == request.CompanyId);

            if (!companyExists)
                return BadRequest("Company does not exist.");

            var user = new User
            {
                Name = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Admin",
                CompanyId = request.CompanyId   // 👈 ahora lo asignamos directo
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User created successfully.");
        }

        // ================= LOGIN =================

        [HttpPost("login")]
        public IActionResult Login(LoginDto model)
        {
            var user = _context.Users
    .IgnoreQueryFilters()
    .FirstOrDefault(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);



            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("CompanyId", user.CompanyId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    role = user.Role,
                    companyId = user.CompanyId
                }
            });
        }
    }
}