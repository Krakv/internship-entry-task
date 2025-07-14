using Microsoft.EntityFrameworkCore;
using Moq;
using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    public class MoveRepositoryTests
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly MoveRepository _repository;
        private readonly Mock<DbSet<Move>> _mockMovesDbSet;

        public MoveRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            _mockContext = new Mock<AppDbContext>(options);
            _mockMovesDbSet = new Mock<DbSet<Move>>();
            _mockContext.Setup(c => c.Moves).Returns(_mockMovesDbSet.Object);
            _repository = new MoveRepository(_mockContext.Object);
        }

        [Fact]
        public async Task AddAsync_AddsMoveToContext()
        {
            var move = new Move { GameId = 1, Player = Player.X, Row = 0, Col = 0 };
            _mockMovesDbSet.Setup(m => m.AddAsync(move, default))
                .ReturnsAsync((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Move>)null);

            var result = await _repository.AddAsync(move);

            Assert.Equal(move, result);
            _mockMovesDbSet.Verify(m => m.AddAsync(move, default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}