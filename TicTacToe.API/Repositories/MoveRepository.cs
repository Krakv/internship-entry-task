using Microsoft.EntityFrameworkCore;
using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    public class MoveRepository : IMoveRepository
    {
        private readonly AppDbContext _context;

        public MoveRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Move> AddAsync(Move move)
        {
            await _context.Moves.AddAsync(move);
            await _context.SaveChangesAsync();
            return move;
        }

        public async Task<List<Move>> GetMovesByGameIdAsync(int gameId)
        {
            return await _context.Moves
                .Where(m => m.GameId == gameId)
                .OrderBy(m => m.MoveTime)
                .ToListAsync();
        }
    }
}
