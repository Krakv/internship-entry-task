using Microsoft.AspNetCore.Http;
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

        [Fact]
        public async Task MakeMove_CachedResult_ReturnsOkWithETag()
        {
            var gameId = 42;
            var moveDto = new MoveDto { Player = Player.O, Row = 1, Col = 2 };
            var expectedEtag = "dummy-etag";
            var expectedResponse = new CreatedMoveDto
            {
                NewBoardState = "--O------",
                Status = GameStatus.InProgress
            };

            _mockMoveService.Setup(s => s.GetCachedMoveResultAsync(gameId, moveDto))
                .ReturnsAsync(new CachedMoveResult
                {
                    Response = expectedResponse,
                    ETag = expectedEtag
                });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.MakeMove(gameId, moveDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);

            Assert.True(_controller.Response.Headers.ContainsKey("ETag"));
            Assert.Equal($"\"{expectedEtag}\"", _controller.Response.Headers.ETag.ToString());
        }

        [Fact]
        public async Task MakeMove_ConcurrentIdenticalRequests_BothReturnSameResponseAndETag()
        {
            var gameId = 77;
            var moveDto = new MoveDto { Player = Player.X, Row = 1, Col = 1 };
            var expectedResponse = new CreatedMoveDto
            {
                NewBoardState = "----X----",
                Status = GameStatus.InProgress
            };

            var callCount = 0;
            string fixedEtag = "some-etag-from-cache";

            _mockMoveService.Setup(s => s.GetCachedMoveResultAsync(gameId, moveDto))
                .ReturnsAsync(() =>
                {
                    if (callCount++ == 0)
                        return null;
                    else
                        return new CachedMoveResult { Response = expectedResponse, ETag = fixedEtag };
                });

            _mockMoveService.Setup(s => s.CacheMoveResultWithCleanupAsync(gameId, moveDto, null, expectedResponse, fixedEtag))
                .Returns(Task.CompletedTask);

            _mockMoveService.Setup(s => s.MakeMoveAsync(gameId, moveDto))
                .ReturnsAsync(new Game(3, 3, 0)
                {
                    BoardState = expectedResponse.NewBoardState,
                    Status = expectedResponse.Status
                });

            var controller1 = new TestMovesController(_mockMoveService.Object, _mockLogger.Object, fixedEtag);
            var controller2 = new TestMovesController(_mockMoveService.Object, _mockLogger.Object, fixedEtag);

            controller1.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller2.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var task1 = controller1.MakeMove(gameId, moveDto);
            var task2 = controller2.MakeMove(gameId, moveDto);
            await Task.WhenAll(task1, task2);

            var result1 = Assert.IsType<OkObjectResult>(task1.Result);
            var result2 = Assert.IsType<OkObjectResult>(task2.Result);

            var dto1 = Assert.IsType<CreatedMoveDto>(result1.Value);
            var dto2 = Assert.IsType<CreatedMoveDto>(result2.Value);

            Assert.Equal(dto1.NewBoardState, dto2.NewBoardState);
            Assert.Equal(dto1.Status, dto2.Status);

            var etag1 = controller1.HttpContext.Response.Headers["ETag"].ToString();
            var etag2 = controller2.HttpContext.Response.Headers["ETag"].ToString();

            Assert.False(string.IsNullOrWhiteSpace(etag1));
            Assert.Equal(etag1, etag2);


        }

        class TestMovesController : MovesController
        {
            private readonly string _etag;

            public TestMovesController(IMoveService moveService, ILogger<MovesController> logger, string etag)
                : base(moveService, logger)
            {
                _etag = etag;
            }

            protected override string GenerateETag(Game game, MoveDto moveDto)
            {
                return _etag;
            }
        }


    }
}