using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class DeliveryDriver
{
    public int EmployeeId { get; set; }

    public string? Licence { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Motorcycle> Motorcycles { get; set; } = new List<Motorcycle>();
}
