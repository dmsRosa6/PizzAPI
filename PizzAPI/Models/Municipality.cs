using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Municipality
{
    public int MunicipalityId { get; set; }

    public string Name { get; set; } = null!;

    public int DistrictId { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual District District { get; set; } = null!;
}
