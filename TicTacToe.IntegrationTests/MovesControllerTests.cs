using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using TicTacToe.API.DTOs;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.IntegrationTests
{
    public class MovesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly Mock<IMoveService> _moveServiceMock;

        public MovesControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _moveServiceMock = factory.MoveServiceMock;
        }

        [Fact]
        public async Task MakeMove_ValidMove_ReturnsOkWithUpdatedBoard()
        {
            int gameId = 1;
            var moveDto = new MoveDto { Player = Player.X, Row = 0, Col = 1 };
            var updatedGame = new Game(3, 3, 0)
            {
                Id = gameId,
                BoardState = "XOX------",
                CurrentPlayer = Player.O,
                Status = GameStatus.InProgress
            };

            _moveServiceMock
                .Setup(m => m.MakeMoveAsync(gameId, It.IsAny<MoveDto>()))
                .ReturnsAsync(updatedGame);

            var content = new StringContent(JsonConvert.SerializeObject(moveDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/api/games/{gameId}/moves", content);

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CreatedMoveDto>(responseBody);

            Assert.Equal(updatedGame.BoardState, result.NewBoardState);
            Assert.Equal(updatedGame.Status, result.Status);
        }

        [Fact]
        public async Task MakeMove_GameNotFound_ReturnsNotFound()
        {
            int gameId = 42;
            var moveDto = new MoveDto { Player = Player.X, Row = 0, Col = 0 };

            _moveServiceMock
                .Setup(m => m.MakeMoveAsync(gameId, It.IsAny<MoveDto>()))
                .ThrowsAsync(new ArgumentException("Game not found"));

            var content = new StringContent(JsonConvert.SerializeObject(moveDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/api/games/{gameId}/moves", content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MakeMove_InvalidOperation_ThrowsBadRequest()
        {
            int gameId = 5;
            var moveDto = new MoveDto { Player = Player.X, Row = 0, Col = 0 };

            _moveServiceMock
                .Setup(m => m.MakeMoveAsync(gameId, It.IsAny<MoveDto>()))
                .ThrowsAsync(new InvalidOperationException("Position already taken"));

            var content = new StringContent(JsonConvert.SerializeObject(moveDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/api/games/{gameId}/moves", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task MakeMove_UnexpectedException_ReturnsInternalServerError()
        {
            int gameId = 10;
            var moveDto = new MoveDto { Player = Player.X, Row = 0, Col = 0 };

            _moveServiceMock
                .Setup(m => m.MakeMoveAsync(gameId, It.IsAny<MoveDto>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var content = new StringContent(JsonConvert.SerializeObject(moveDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/api/games/{gameId}/moves", content);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
