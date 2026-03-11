namespace SaaS.Api.DTOs.Products
{
    public class UpdateProductDto
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public bool IsActive { get; set; }
    }
}
