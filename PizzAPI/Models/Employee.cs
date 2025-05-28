using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Salary { get; set; }

    public int StoreId { get; set; }

    public bool IsActive { get; set; }

    public virtual DeliveryDriver? DeliveryDriver { get; set; }

    public virtual Store Store { get; set; } = null!;

    public virtual StoreStaff? StoreStaff { get; set; }
}
