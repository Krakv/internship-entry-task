using Microsoft.EntityFrameworkCore.Storage;
using TicTacToe.API.DTOs;

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

        /// <summary>
        /// Сохраняет информацию о последнем выполненном ходе для указанной игры.
        /// Используется для проверки повторных идентичных ходов.
        /// </summary>
        /// <param name="gameId">Идентификатор игры</param>
        /// <param name="move">Данные хода (координаты и игрок)</param>
        /// <returns>Task, представляющий асинхронную операцию</returns>
        Task SetPreviousMoveAsync(int gameId, MoveDto move);

        /// <summary>
        /// Получает последний сохранённый ход для указанной игры.
        /// </summary>
        /// <param name="gameId">Идентификатор игры</param>
        /// <returns>
        /// Данные последнего хода или null, если ход не найден или истёк срок хранения
        /// </returns>
        Task<MoveDto?> GetPreviousMoveAsync(int gameId);

        /// <summary>
        /// Удаляет информацию о последнем ходе для указанной игры.
        /// </summary>
        /// <param name="gameId">Идентификатор игры</param>
        /// <returns>Task, представляющий асинхронную операцию</returns>
        Task ClearPreviousMoveAsync(int gameId);

    }
}
