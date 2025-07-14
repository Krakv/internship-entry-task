using TicTacToe.API.Models;
using TicTacToe.API.Repositories;

namespace TicTacToe.API.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly GameFactory _gameFactory;

        public GameService(IGameRepository gameRepository, GameFactory gameFactory)
        {
            _gameRepository = gameRepository;
            _gameFactory = gameFactory;
        }

        public async Task<Game> CreateGameAsync()
        {
            var game = _gameFactory.CreateGame();
            return await _gameRepository.AddAsync(game);
        }

        public async Task<Game> GetGameAsync(int id)
        {
            return await _gameRepository.GetByIdAsync(id);
        }
    }
}
