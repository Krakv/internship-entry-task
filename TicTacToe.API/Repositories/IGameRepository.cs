using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    public interface IGameRepository
    {
        Task<Game?> GetByIdAsync(int id);
        Task<Game> AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task SaveChangesAsync();
    }
}
