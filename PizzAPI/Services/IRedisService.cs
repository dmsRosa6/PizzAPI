using PizzAPI.Models;
using PizzAPI.Models.ValueObjects;

namespace PizzAPI.Services
{

    public interface IRedisService
    {
        Task<bool> StartOrderAsync(int orderId);
        Task<string?> GetOrderStatusAsync(int orderId);
        Task<Coordinates?> GetOrderCoordinatesAsync(int orderId);
        Task PublishOrderChangedAsync(int orderId, string? status, int? latitute, int? longitude);
        Task SubscribeOrderChangedAsync(int orderId, Func<OrderUpdate, Task> handler, CancellationToken cancelToken);
    }
}