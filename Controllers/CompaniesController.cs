using Microsoft.AspNetCore.Mvc;
using SaaS.Api.Data;
using SaaS.Api.Models;

namespace SaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return Ok(company);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Companies.ToList());
        }
    }
}