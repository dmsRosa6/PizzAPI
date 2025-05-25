using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class TakeawayOrder
{
    public int OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;
}
