using Microsoft.AspNetCore.Mvc;
using Moq;
using TicTacToe.API.DTOs;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers
{
    public class GamesControllerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly GamesController _controller;

        public GamesControllerTests()
        {
            _mockGameService = new Mock<IGameService>();
            _controller = new GamesController(_mockGameService.Object);
        }

        [Fact]
        public async Task GetGame_WithExistingId_ReturnsGameDto()
        {
            var gameId = 1;
            var expectedGame = new Game(3, 3, 0)
            {
                BoardState = "X--------",
                CurrentPlayer = Player.O,
                Status = GameStatus.InProgress
            };

            _mockGameService.Setup(s => s.GetGameAsync(gameId))
                .ReturnsAsync(expectedGame);

            var result = await _controller.GetGame(gameId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<GameDto>(okResult.Value);

            Assert.Equal(expectedGame.BoardState, dto.BoardState);
            Assert.Equal(expectedGame.CurrentPlayer, dto.CurrentPlayer);
            Assert.Equal(expectedGame.Status, dto.Status);
        }

        [Fact]
        public async Task GetGame_WithNonExistingId_ReturnsNotFound()
        {
            var gameId = 999;
            _mockGameService.Setup(s => s.GetGameAsync(gameId))
                .ReturnsAsync((Game)null);

            var result = await _controller.GetGame(gameId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetGame_WithServiceException_ReturnsBadRequest()
        {
            var gameId = 1;
            _mockGameService.Setup(s => s.GetGameAsync(gameId))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.GetGame(gameId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Test exception", badRequestResult.Value);
        }

        [Fact]
        public async Task PostGame_CreatesNewGame_ReturnsCreatedGameDto()
        {
            var expectedGame = new Game(3, 3, 0)
            {
                Id = 1,
                BoardSize = 3,
                Status = GameStatus.InProgress
            };

            _mockGameService.Setup(s => s.CreateGameAsync())
                .ReturnsAsync(expectedGame);

            var result = await _controller.PostGame();

            var createdAtResult = Assert.IsType<CreatedResult>(result);
            var dto = Assert.IsType<CreatedGameDto>(createdAtResult.Value);

            Assert.Equal(expectedGame.Id, dto.Id);
            Assert.Equal(expectedGame.BoardSize, dto.BoardSize);
            Assert.Equal(expectedGame.Status, dto.Status);
            Assert.Equal($"/api/games/{expectedGame.Id}", createdAtResult.Location);
        }

        [Fact]
        public async Task PostGame_WithServiceException_ReturnsBadRequest()
        {
            _mockGameService.Setup(s => s.CreateGameAsync())
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.PostGame();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Test exception", badRequestResult.Value);
        }
    }
}
