using Microsoft.EntityFrameworkCore;
using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с ходами
    /// </summary>
    public class MoveRepository : IMoveRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public MoveRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<Move> AddAsync(Move move)
        {
            await _context.Moves.AddAsync(move);
            await _context.SaveChangesAsync();
            return move;
        }

        /// <inheritdoc/>
        public async Task<List<Move>> GetMovesByGameIdAsync(int gameId)
        {
            return await _context.Moves
                .Where(m => m.GameId == gameId)
                .OrderBy(m => m.MoveTime)
                .ToListAsync();
        }
    }
}
