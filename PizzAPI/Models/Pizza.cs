using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Pizza
{
    public int PizzaId { get; set; }

    public string Name { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
