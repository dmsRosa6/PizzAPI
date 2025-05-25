using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string ItemType { get; set; } = null!;

    public int ItemId { get; set; }

    public decimal DiscountPercent { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }
}
