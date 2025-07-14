using Microsoft.EntityFrameworkCore;
using Moq;
using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    public class GameRepositoryTests
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly GameRepository _repository;
        private readonly Mock<DbSet<Game>> _mockGamesDbSet;

        public GameRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            _mockContext = new Mock<AppDbContext>(options);
            _mockGamesDbSet = new Mock<DbSet<Game>>();
            _mockContext.Setup(c => c.Games).Returns(_mockGamesDbSet.Object);
            _repository = new GameRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsGame()
        {
            var gameId = 1;
            var expectedGame = new Game(3, 3, 0) { Id = gameId };

            _mockGamesDbSet.Setup(g => g.FindAsync(gameId))
                .ReturnsAsync(expectedGame);

            var result = await _repository.GetByIdAsync(gameId);

            Assert.Equal(expectedGame, result);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
        {
            var gameId = 999;
            _mockGamesDbSet.Setup(g => g.FindAsync(gameId))
                .ReturnsAsync((Game)null);

            var result = await _repository.GetByIdAsync(gameId);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_AddsGameToContext()
        {
            var game = new Game(3, 3, 0);
            _mockGamesDbSet.Setup(g => g.AddAsync(game, default))
                .ReturnsAsync((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Game>)null);

            var result = await _repository.AddAsync(game);

            Assert.Equal(game, result);
            _mockGamesDbSet.Verify(g => g.AddAsync(game, default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesGameInContext()
        {
            var game = new Game(3, 3, 0);

            await _repository.UpdateAsync(game);

            _mockGamesDbSet.Verify(g => g.Update(game), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task SaveChangesAsync_CallsContextSaveChanges()
        {
            await _repository.SaveChangesAsync();

            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}
