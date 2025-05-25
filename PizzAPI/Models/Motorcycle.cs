using System;
using System.Collections.Generic;

namespace PizzAPI.Models;

public partial class Motorcycle
{
    public int MotorcycleId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string? Brand { get; set; }

    public int DriverId { get; set; }

    public virtual DeliveryDriver Driver { get; set; } = null!;
}
