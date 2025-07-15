using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с играми
    /// </summary>
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public GameRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<Game> AddAsync(Game game)
        {
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
            return game;
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
