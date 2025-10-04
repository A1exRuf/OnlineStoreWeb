using Application.Abstractions;
using Application.Abstractions.Carts;
using Application.Dtos.Cart;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

public class GuestCartStorage : IGuestCartStorage
{
    private readonly IDatabase _redis;
    private readonly JsonSerializerOptions _options;
    private readonly ICurrentUserService _currentUserService;

    public GuestCartStorage(
        IDatabase redis,
        ICurrentUserService currentUserService)
    {
        _redis = redis;
        _options = new JsonSerializerOptions { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        _currentUserService = currentUserService;
    }

    public async Task<GuestCartDto?> GetCartAsync()
    {
        var id = _currentUserService.GuestCartId;

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

    public async Task DeleteCartAsync()
    {
        var id = _currentUserService.GuestCartId;

        await _redis.KeyDeleteAsync(GetKey(id));
    }

    private static string GetKey(Guid id) => $"guest_cart:{id}";

    public async Task TouchAsync()
    {
        var id = _currentUserService.GuestCartId;

        await _redis.KeyExpireAsync(
            GetKey(id), 
            TimeSpan.FromDays(7));
    }
}
