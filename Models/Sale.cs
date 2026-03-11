using SaaS.Api.Common;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models
{
    public class Sale : ITenantEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ClientsId { get; set; }
        public Clients Clients { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

        public Guid CompanyId { get; set; } // Multi-tenant

        public List<SaleItem> Items { get; set; } = new();
    }

    public class SaleItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ProductsId { get; set; }
        public Products Products { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal Subtotal => Quantity * UnitPrice;
    }
}