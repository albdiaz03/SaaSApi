using SaaS.Api.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models
{
    public class Clients : ITenantEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // clave primaria

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        public bool IsActive { get; set; } = true;

        // Multi-tenant
        public Guid CompanyId { get; set; }
    }
}