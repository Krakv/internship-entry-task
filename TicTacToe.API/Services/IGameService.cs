using TicTacToe.API.Models;

namespace TicTacToe.API.Services
{
    public interface IGameService
    {
        Task<Game> CreateGameAsync();
        Task<Game> GetGameAsync(int id);
    }
}
