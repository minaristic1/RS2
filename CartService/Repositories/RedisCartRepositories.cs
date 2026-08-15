using System.Text.Json;
using CartService.Domain1;
using StackExchange.Redis;

namespace CartService.Repositories;

public class RedisCartRepositories : ICartRepository
{
    private readonly IDatabase _database;

    public RedisCartRepositories(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task<Cart?> GetCartAsync(Guid userId)
    {
        var key = GetCartKey(userId);
        var value = await _database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Cart>(value.ToString());
    }

    public async Task<Cart> SaveCartAsync(Cart cart)
    {
        var key = GetCartKey(cart.UserId);
        var value = JsonSerializer.Serialize(cart);
        await _database.StringSetAsync(key, value);

        return cart;
    }

    public async Task DeleteCartAsync(Guid userId)
    {
        var key = GetCartKey(userId);
        await _database.KeyDeleteAsync(key);
    }

    private static string GetCartKey(Guid userId)
    {
        return $"cart:{userId}";
    }
}