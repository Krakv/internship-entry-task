using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    /// <summary>
    /// Репозиторий для работы с ходами
    /// </summary>
    public interface IMoveRepository
    {
        /// <summary>
        /// Добавить новый ход
        /// </summary>
        /// <param name="move">Добавляемый ход</param>
        /// <returns>Добавленный ход</returns>
        Task<Move> AddAsync(Move move);

        /// <summary>
        /// Получить все ходы для указанной игры
        /// </summary>
        /// <param name="gameId">Идентификатор игры</param>
        /// <returns>Список ходов</returns>
        Task<List<Move>> GetMovesByGameIdAsync(int gameId);
    }
}
