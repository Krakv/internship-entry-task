using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TicTacToe.API.DTOs;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers
{
    public class MovesControllerTests
    {
        private readonly Mock<IMoveService> _mockMoveService;
        private readonly Mock<ILogger<MovesController>> _mockLogger;
        private readonly MovesController _controller;

        public MovesControllerTests()
        {
            _mockMoveService = new Mock<IMoveService>();
            _mockLogger = new Mock<ILogger<MovesController>>();

            _controller = new MovesController(_mockMoveService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task MakeMove_FinishedGame_ReturnsConflict()
        {
            var gameId = 1;
            var moveDto = new MoveDto { Player = Player.X, Row = 0, Col = 0 };
            var game = new Game(3, 3, 0)
            {
                BoardState = "XXX------",
                Status = GameStatus.XWon
            };

            _mockMoveService.Setup(s => s.MakeMoveAsync(gameId, moveDto))
                .ReturnsAsync(game);

            var result = await _controller.MakeMove(gameId, moveDto);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Game has been finished", conflictResult.Value);
        }

        [Fact]
        public async Task MakeMove_GameNotFound_ReturnsNotFound()
        {
            var gameId = 999;
            var moveDto = new MoveDto { Player = Player.X, Row = 0, Col = 0 };

            _mockMoveService.Setup(s => s.MakeMoveAsync(gameId, moveDto))
                .ThrowsAsync(new ArgumentException("Game not found"));

            var result = await _controller.MakeMove(gameId, moveDto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task MakeMove_InvalidMove_ReturnsBadRequest()
        {
            var gameId = 1;
            var moveDto = new MoveDto { Player = Player.X, Row = 0, Col = 0 };

            _mockMoveService.Setup(s => s.MakeMoveAsync(gameId, moveDto))
                .ThrowsAsync(new InvalidOperationException("Invalid move"));

            var result = await _controller.MakeMove(gameId, moveDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task MakeMove_ServerError_ReturnsStatusCode500()
        {
            var gameId = 1;
            var moveDto = new MoveDto { Player = Player.X, Row = 0, Col = 0 };

            _mockMoveService.Setup(s => s.MakeMoveAsync(gameId, moveDto))
                .ThrowsAsync(new Exception("Server error"));

            var result = await _controller.MakeMove(gameId, moveDto);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}