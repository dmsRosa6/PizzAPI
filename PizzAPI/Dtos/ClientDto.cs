using PizzAPI.Models;

namespace PizzAPI.Dtos
{
    public class ClientDto
    {
        public int ClientId { get; set; }

        public string Name { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Nif { get; set; }

        public AddressDto? Address { get; set; }

        public static ClientDto FromEntity(Client client) => new ClientDto
        {
            ClientId = client.ClientId,
            Name = client.Name,
            PhoneNumber = client.PhoneNumber,
            Nif = client.Nif,
            Address = AddressDto.FromEntity(client.Address)
        };
    }
}