using StackExchange.Redis;
using System.Text.Json;

namespace TicTacToe.API.Services
{
    /// <summary>
    /// Сервис кэширования на Redis
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _redis;

        /// <summary>
        /// Инициализирует сервис кэширования
        /// </summary>
        /// <param name="redis">Подключение к Redis</param>
        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = redis.GetDatabase();
        }

        /// <inheritdoc/>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }

        /// <inheritdoc/>
        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _db.StringGetAsync(key);
            return json.HasValue
                ? JsonSerializer.Deserialize<T>(json)
                : default;
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }

}
