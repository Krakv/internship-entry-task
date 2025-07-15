using TicTacToe.API.Models;
using TicTacToe.API.Repositories;

namespace TicTacToe.API.Services
{
    /// <summary>
    /// Реализация сервиса для работы с играми
    /// </summary>
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly GameFactory _gameFactory;

        /// <summary>
        /// Инициализирует сервис игр
        /// </summary>
        /// <param name="gameRepository">Репозиторий игр</param>
        /// <param name="gameFactory">Фабрика игр</param>
        public GameService(IGameRepository gameRepository, GameFactory gameFactory)
        {
            _gameRepository = gameRepository;
            _gameFactory = gameFactory;
        }

        /// <inheritdoc/>
        public async Task<Game> CreateGameAsync()
        {
            var game = _gameFactory.CreateGame();
            return await _gameRepository.AddAsync(game);
        }

        /// <inheritdoc/>
        public async Task<Game> GetGameAsync(int id)
        {
            return await _gameRepository.GetByIdAsync(id);
        }
    }
}
