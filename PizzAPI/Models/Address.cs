using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public string StreetName { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string? DoorNumber { get; set; }

    public int MunicipalityId { get; set; }

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual Municipality Municipality { get; set; } = null!;
}
