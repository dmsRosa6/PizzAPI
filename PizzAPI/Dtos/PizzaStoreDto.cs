namespace PizzAPI.Dtos
{
    public class PizzaStoreDto
    {
        public int StoreId { get; set; }

        public string Name { get; set; } = null!;

        public AddressDto Address { get; set; } = null!;
    }
}