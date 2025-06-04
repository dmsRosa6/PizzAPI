using PizzAPI.Models;

namespace PizzAPI.Dtos
{
    public class ClientCreateRequestDto
    {
        public string Name { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Nif { get; set; }
        public AddressCreateRequestDto? Address { get; set; }

        public Client ToEntity(Address address)
        {
            return new Client
            {
                Name = Name,
                PhoneNumber = PhoneNumber,
                Nif = Nif,
                AddressId = address.AddressId
            };
        }

        public Client ToEntity(int addressId)
        {
            return new Client
            {
                Name = Name,
                PhoneNumber = PhoneNumber,
                Nif = Nif,
                AddressId = addressId
            };
        }
    }
}
