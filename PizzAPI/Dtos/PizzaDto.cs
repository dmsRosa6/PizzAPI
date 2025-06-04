using PizzAPI.Models;

namespace PizzAPI.Dtos
{
    public class PizzaDto
    {
        public int PizzaId { get; set; }

        public string Name { get; set; } = null!;

        public decimal BasePrice { get; set; }

        public List<ProductDto> Products { get; set; } = [];
    }
}