using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Delivery
{
    public int OrderId { get; set; }

    public int AddressId { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
