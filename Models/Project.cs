using SaaS.Api.Common;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models
{
    public class Project : ITenantEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        public Guid CompanyId { get; set; }
    }
}