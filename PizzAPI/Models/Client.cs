﻿using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string Name { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Nif { get; set; }

    public int? AddressId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
