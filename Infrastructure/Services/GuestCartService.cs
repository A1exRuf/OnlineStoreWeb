using Application.Abstractions;
using Application.Dtos.Cart;
using Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

public class GuestCartService : IGuestCartService
{
    private readonly IDatabase _redis;
    private readonly JsonSerializerOptions _options;

    public GuestCartService(IDatabase redis)
    {
        _redis = redis;
        _options = new JsonSerializerOptions { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }

    public async Task<GuestCartDto?> GetCartAsync(Guid id)
    {
        var json = await _redis.StringGetAsync(GetKey(id));

        if (json.HasValue)
            return JsonSerializer.Deserialize<GuestCartDto>(json!, _options);

        return  null;
    }

    public async Task SaveCartAsync(GuestCartDto cart)
    {
        var json = JsonSerializer.Serialize(cart, _options);

        await _redis.StringSetAsync(
            GetKey(cart.Id),
            json,
            TimeSpan.FromDays(7));
    }

    public async Task DeleteCartAsync(Guid id)
    {
        await _redis.KeyDeleteAsync(GetKey(id));
    }

    private static string GetKey(Guid id) => $"guest_cart:{id}";

    public async Task TouchAsync(Guid id)
    {
        await _redis.KeyExpireAsync(
            GetKey(id), 
            TimeSpan.FromDays(7));
    }
}
