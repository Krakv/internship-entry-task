using Microsoft.Extensions.Options;
using Moq;
using TicTacToe.API.Models;
using TicTacToe.API.Repositories;

namespace TicTacToe.API.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _mockRepository;
        private readonly Mock<GameFactory> _mockFactory;
        private readonly GameService _service;

        public GameServiceTests()
        {
            _mockRepository = new Mock<IGameRepository>();
            _mockFactory = new Mock<GameFactory>(Mock.Of<IOptions<GameSettings>>());
            _service = new GameService(_mockRepository.Object, _mockFactory.Object);
        }

        [Fact]
        public async Task CreateGameAsync_CreatesAndSavesGame()
        {
            var expectedGame = new Game(3, 3, 0);
            _mockFactory.Setup(f => f.CreateGame())
                .Returns(expectedGame);
            _mockRepository.Setup(r => r.AddAsync(expectedGame))
                .ReturnsAsync(expectedGame);

            var result = await _service.CreateGameAsync();

            Assert.Equal(expectedGame, result);
            _mockFactory.Verify(f => f.CreateGame(), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(expectedGame), Times.Once);
        }

        [Fact]
        public async Task GetGameAsync_WithExistingId_ReturnsGame()
        {
            var gameId = 1;
            var expectedGame = new Game(3, 3, 0);
            _mockRepository.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(expectedGame);

            var result = await _service.GetGameAsync(gameId);

            Assert.Equal(expectedGame, result);
            _mockRepository.Verify(r => r.GetByIdAsync(gameId), Times.Once);
        }

        [Fact]
        public async Task GetGameAsync_WithNonExistingId_ReturnsNull()
        {
            var gameId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync((Game)null);

            var result = await _service.GetGameAsync(gameId);

            Assert.Null(result);
            _mockRepository.Verify(r => r.GetByIdAsync(gameId), Times.Once);
        }
    }
}
