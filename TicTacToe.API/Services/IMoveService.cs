using TicTacToe.API.DTOs;
using TicTacToe.API.Models;

namespace TicTacToe.API.Services
{
    /// <summary>
    /// Сервис для работы с ходами
    /// </summary>
    public interface IMoveService
    {
        /// <summary>
        /// Совершает ход в указанной игре
        /// </summary>
        /// <param name="gameId">ID игры</param>
        /// <param name="moveDto">Данные хода</param>
        /// <returns>Обновленное состояние игры</returns>
        Task<Game> MakeMoveAsync(int gameId, MoveDto moveDto);

        /// <summary>
        /// Получает кэшированный результат хода
        /// </summary>
        /// <param name="gameId">ID игры</param>
        /// <param name="moveDto">Данные хода</param>
        /// <returns>Кэшированный результат или null</returns>
        Task<CachedMoveResult?> GetCachedMoveResultAsync(int gameId, MoveDto moveDto);

        /// <summary>
        /// Кэширует результат хода
        /// </summary>
        /// <param name="gameId">ID игры</param>
        /// <param name="moveDto">Данные хода</param>
        /// <param name="response">Результат хода</param>
        /// <param name="etag">ETag для валидации</param>
        Task CacheMoveResultAsync(int gameId, MoveDto moveDto, CreatedMoveDto response, string etag);

        /// <summary>
        /// Проверяет валидность хода
        /// </summary>
        /// <param name="game">Игра</param>
        /// <param name="moveDto">Ход</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>True если ход валиден</returns>
        bool IsValidMove(Game game, MoveDto moveDto, out string message);
    }
}
