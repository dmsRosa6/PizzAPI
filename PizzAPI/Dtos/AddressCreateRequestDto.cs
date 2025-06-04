using PizzAPI.Models;

namespace PizzAPI.Dtos
{
    public class AddressCreateRequestDto
    {
        public string StreetName { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string? DoorNumber { get; set; }
        public int MunicipalityId { get; set; }
        public int DistrictId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Address ToEntity()
        {
            return new Address
            {
                StreetName = StreetName,
                PostalCode = PostalCode,
                DoorNumber = DoorNumber,
                MunicipalityId = MunicipalityId,
                Latitude = Latitude,
                Longitude = Longitude
            };
        }
    }
}
