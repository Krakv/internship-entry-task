using TicTacToe.API.Models;

namespace TicTacToe.API.Services
{
    /// <summary>
    /// Сервис для работы с играми
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Создает новую игру
        /// </summary>
        /// <returns>Созданная игра</returns>
        Task<Game> CreateGameAsync();

        /// <summary>
        /// Получает игру по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор игры</param>
        /// <returns>Найденная игра</returns>
        Task<Game> GetGameAsync(int id);
    }
}
