using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    /// <summary>
    /// Репозиторий для работы с играми
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// Получить игру по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор игры</param>
        /// <returns>Найденная игра или null</returns>
        Task<Game?> GetByIdAsync(int id);

        /// <summary>
        /// Добавить новую игру
        /// </summary>
        /// <param name="game">Добавляемая игра</param>
        /// <returns>Добавленная игра</returns>
        Task<Game> AddAsync(Game game);

        /// <summary>
        /// Обновить данные игры
        /// </summary>
        /// <param name="game">Игра для обновления</param>
        Task UpdateAsync(Game game);

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        Task SaveChangesAsync();
    }
}
