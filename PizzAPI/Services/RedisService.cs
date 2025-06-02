
using System.Text.Json;
using System.Text.RegularExpressions;
using PizzAPI.Models;
using PizzAPI.Models.ValueObjects;
using StackExchange.Redis;

namespace PizzAPI.Services
{
    public class RedisService(IConnectionMultiplexer mux) : IRedisService
    {

        private readonly ISubscriber _pubSub = mux.GetSubscriber();
        private readonly IDatabase _db = mux.GetDatabase();

        private const string DELIMITER = ":";
        private const string STATUS_KEYWORD = ".status";
        private const string LOCATION_KEYWORD = ".location";


        private const string CREATE_ORDER_FUN = "create_order";
        private const string UPDATE_ORDER_FUN = "update_order";
        private const string END_ORDER_STATUS_FUN = "end_order";
        private const string GET_ORDER_STATUS_FUN = "get_order";

        public async Task<bool> StartOrderAsync(int orderId)
        {
            RedisResult result = await _db.ExecuteAsync(
                "FCALL",
                CREATE_ORDER_FUN,
                1,
                orderId.ToString(),
                OrderStatus.Started.ToString()
            );

            return result.Resp2Type == ResultType.SimpleString && (string)result! == "OK";
        }

        public async Task<bool> EndOrderAsync(int orderId)
        {
            RedisResult result = await _db.ExecuteAsync(
                "FCALL",
                END_ORDER_STATUS_FUN,
                1,
                orderId.ToString()
            );

            return result.Resp2Type == ResultType.SimpleString && (string)result! == "OK";
        }

        public async Task<string?> GetOrderStatusAsync(int orderId)
        {
            var key = $"{orderId}{STATUS_KEYWORD}";
            var val = await _db.StringGetAsync(key);
            return val.HasValue ? val.ToString() : null;

        }

        public async Task<Coordinates?> GetOrderCoordinatesAsync(int orderId)
        {
            var key = $"{orderId}{LOCATION_KEYWORD}";
            var val = await _db.StringGetAsync(key);

            if (val.IsNullOrEmpty)
                return null;

            var parts = val.ToString().Split(DELIMITER);
            if (parts.Length != 2)
                return null;

            if (double.TryParse(parts[0], out var lat) &&
                double.TryParse(parts[1], out var lon))
            {
                return new Coordinates()
                {
                    Latitude = lat,
                    Longitude = lon
                };
            }

            return null;
        }

        public async Task PublishOrderChangedAsync(int orderId, string? status, int? latitude, int? longitude)
        {

            await _db.ExecuteAsync(
                "FCALL",
                UPDATE_ORDER_FUN,
                1,
                orderId.ToString(),
                status ?? "",
                latitude == null ? "" : latitude.ToString()!,
                longitude == null ? "" : longitude.ToString()!
            );
        }

       public async Task SubscribeOrderChangedAsync(int orderId, Func<OrderUpdate, Task> handler, CancellationToken cancelToken)
        {
            string channelName = $"{orderId}.chan";

            await _pubSub.SubscribeAsync(new RedisChannel(channelName, RedisChannel.PatternMode.Literal), 
                async (chan, rawMessage) =>
            {
                try
                {
                    if (cancelToken.IsCancellationRequested) return;
                    
                    var update = JsonSerializer.Deserialize<OrderUpdate>(rawMessage!);
                    if (update is not null)
                    {
                        await handler(update);
                        return;
                    }
                }
                catch
                {
                    //TODO Malformed JSON or another problem, maybe do something
                }

                // FALLBACK for whatever reason 
                var rawResult = await _db.ExecuteAsync(
                    "FCALL",
                    GET_ORDER_STATUS_FUN,
                    1,
                    orderId.ToString()
                );

                if (rawResult is not RedisResult redisResult || 
                    string.IsNullOrEmpty(redisResult.ToString()))
                {
                    throw new InvalidOperationException($"Redis returned no data for order {orderId}");
                }

                var rawJson = redisResult.ToString()!;

                OrderUpdate fallbackUpdate;
                try
                {
                    fallbackUpdate = JsonSerializer
                        .Deserialize<OrderUpdate>(rawJson)
                        ?? throw new JsonException("Deserialized OrderUpdate was null.");
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException("Failed to parse fallback JSON.", ex);
                }

                await handler(fallbackUpdate);

            });
        }

        
    }
}