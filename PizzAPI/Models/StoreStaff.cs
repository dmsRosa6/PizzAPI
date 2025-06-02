using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class StoreStaff
{
    public int EmployeeId { get; set; }

    public string Role { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
