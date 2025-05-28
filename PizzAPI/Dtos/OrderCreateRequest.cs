namespace PizzAPI.Dtos
{
    public class OrderCreateRequest
    {
        public int ClientId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public int? AddressId { get; set; }
    }
}
