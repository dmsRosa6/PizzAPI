using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int ClientId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Delivery? Delivery { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual TakeawayOrder? TakeawayOrder { get; set; }
}
