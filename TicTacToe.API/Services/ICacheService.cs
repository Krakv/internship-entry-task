namespace TicTacToe.API.Services
{
    /// <summary>
    /// Интерфейс сервиса кэширования
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Устанавливает значение в кэш
        /// </summary>
        /// <typeparam name="T">Тип сохраняемого значения</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <param name="value">Сохраняемое значение</param>
        /// <param name="expiry">Время жизни записи в кэше (null - без ограничения)</param>
        /// <returns>Task</returns>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// Получает значение из кэша
        /// </summary>
        /// <typeparam name="T">Тип получаемого значения</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <returns>Значение из кэша или null, если не найдено</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Удаляет значение из кэша
        /// </summary>
        /// <param name="key">Ключ кэша</param>
        /// <returns>Task</returns>
        Task RemoveAsync(string key);
    }
}
