namespace PizzAPI.Dtos
{
    public class Employee
    {
        public string Name { get; set; } = null!;

        public decimal? Salary { get; set; }

        public int StoreId { get; set; }

    }
}