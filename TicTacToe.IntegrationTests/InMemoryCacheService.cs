using System.Text.Json;
using TicTacToe.API.DTOs;
using TicTacToe.API.Services;

namespace TicTacToe.IntegrationTests
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, (string value, DateTimeOffset? expiry)> _store = new();

        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            DateTimeOffset? expiration = expiry.HasValue ? DateTimeOffset.UtcNow.Add(expiry.Value) : null;
            _store[key] = (json, expiration);
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_store.TryGetValue(key, out var entry))
            {
                if (entry.expiry == null || entry.expiry > DateTimeOffset.UtcNow)
                {
                    return Task.FromResult(JsonSerializer.Deserialize<T>(entry.value));
                }
                _store.Remove(key);
            }
            return Task.FromResult<T?>(default);
        }

        public Task RemoveAsync(string key)
        {
            _store.Remove(key);
            return Task.CompletedTask;
        }

        public async Task SetPreviousMoveAsync(int gameId, MoveDto move)
        {
            await SetAsync($"game:{gameId}:last_move", move, TimeSpan.FromHours(1));
        }

        public async Task<MoveDto?> GetPreviousMoveAsync(int gameId)
        {
            return await GetAsync<MoveDto>($"game:{gameId}:last_move");
        }

        public async Task ClearPreviousMoveAsync(int gameId)
        {
            await RemoveAsync($"game:{gameId}:last_move");
        }

        public void CleanExpiredEntries()
        {
            var now = DateTimeOffset.UtcNow;
            var expiredKeys = new List<string>();

            foreach (var kvp in _store)
            {
                if (kvp.Value.expiry != null && kvp.Value.expiry <= now)
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                _store.Remove(key);
            }
        }
    }
}