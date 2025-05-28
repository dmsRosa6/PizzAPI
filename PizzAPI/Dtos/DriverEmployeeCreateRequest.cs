using PizzAPI.Models;

namespace PizzAPI.Dtos
{
    public class DriverEmployeeCreateRequest
    {

        public string Name { get; set; } = null!;

        public decimal? Salary { get; set; }

        public int StoreId { get; set; }

        public bool IsActive { get; set; }

        public string? Licence { get; set; } = null!;

    }
}