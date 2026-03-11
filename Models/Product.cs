using SaaS.Api.Common;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models
{
    public class Products : ITenantEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid CompanyId { get; set; } // Multi-tenant
    }
}