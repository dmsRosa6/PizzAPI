using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace PizzAPI.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public string StreetName { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string? DoorNumber { get; set; }

    public int MunicipalityId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    [Column("geom")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public Point Geom { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual Municipality Municipality { get; set; } = null!;

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
