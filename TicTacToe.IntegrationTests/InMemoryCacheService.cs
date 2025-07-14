using System.Text.Json;
using TicTacToe.API.Services;

namespace TicTacToe.IntegrationTests
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, (string value, DateTimeOffset? expiry)> _store = new();

        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            var expiration = expiry.HasValue ? DateTimeOffset.UtcNow.Add(expiry.Value) : (DateTimeOffset?)null;
            _store[key] = (json, expiration);
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_store.TryGetValue(key, out var entry))
            {
                if (entry.expiry == null || entry.expiry > DateTimeOffset.UtcNow)
                {
                    var obj = JsonSerializer.Deserialize<T>(entry.value);
                    return Task.FromResult<T?>(obj);
                }
                else
                {
                    _store.Remove(key);
                }
            }

            return Task.FromResult<T?>(default);
        }

        public Task RemoveAsync(string key)
        {
            _store.Remove(key);
            return Task.CompletedTask;
        }
    }
}
