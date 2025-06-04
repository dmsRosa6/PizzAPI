using PizzAPI.Models;

namespace PizzAPI.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public ProductType Type { get; set; }
    }
}