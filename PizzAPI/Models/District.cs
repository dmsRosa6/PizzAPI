using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class District
{
    public int DistrictId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>();
}
