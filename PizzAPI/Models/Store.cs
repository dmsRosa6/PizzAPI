using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Store
{
    public int StoreId { get; set; }

    public int AddressId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
