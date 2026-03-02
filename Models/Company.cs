using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        public string Plan { get; set; } = "Free";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}