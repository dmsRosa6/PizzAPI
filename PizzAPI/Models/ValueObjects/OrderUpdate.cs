using System.Text.Json.Serialization;
using PizzAPI.Services;

namespace PizzAPI.Models.ValueObjects
{
    public class OrderUpdate
{
    [JsonPropertyName("message")]
    public required OrderUpdateMessages Message { get; set; }

    [JsonPropertyName("orderId")]
    public required int OrderId { get; set; }

    [JsonPropertyName("orderStatus")]
    public required OrderStatus OrderStatus { get; set; }

    [JsonPropertyName("coordinates")]
    public required Coordinates Coordinates { get; set; }
}
}