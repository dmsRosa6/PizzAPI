namespace PizzAPI.Dtos
{
    public class CombinedEmployeeCreateRequest
    {
        public string Name { get; set; } = null!;

        public decimal? Salary { get; set; }

        public int StoreId { get; set; }

        public bool IsActive { get; set; }

        public string Role { get; set; } = null!;

        public string? Licence { get; set; } = null!;
    }
}