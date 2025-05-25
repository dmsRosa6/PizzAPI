using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class OrderItem
{
    public int OrderId { get; set; }

    public int PizzaId { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Pizza Pizza { get; set; } = null!;
}
