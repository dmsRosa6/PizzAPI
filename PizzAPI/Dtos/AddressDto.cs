
using PizzAPI.Models;

namespace PizzAPI.Dtos{
    public class AddressDto
    {
        public int AddressId { get; set; }
        public string StreetName { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string? DoorNumber { get; set; }
        public string MunicipalityName { get; set; } = null!;
        public string DistrictName { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static AddressDto FromEntity(Address address)
        {
            return new AddressDto
            {
                AddressId = address.AddressId,
                StreetName = address.StreetName,
                PostalCode = address.PostalCode,
                DoorNumber = address.DoorNumber,
                Latitude = address.Latitude,
                Longitude = address.Longitude,
                MunicipalityName = address.Municipality?.Name ?? string.Empty,  
                DistrictName = address.Municipality?.District?.Name ?? string.Empty
            };
        }
        
    }
}