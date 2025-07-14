using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    public interface IMoveRepository
    {
        Task<Move> AddAsync(Move move);
        Task<List<Move>> GetMovesByGameIdAsync(int gameId);
    }
}
