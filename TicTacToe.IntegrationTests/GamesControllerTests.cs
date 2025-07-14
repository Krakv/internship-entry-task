using Moq;
using Newtonsoft.Json;
using System.Net;
using TicTacToe.API.DTOs;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.IntegrationTests
{
    public class GamesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly Mock<IGameService> _gameServiceMock;

        public GamesControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _gameServiceMock = factory.GameServiceMock;
        }

        [Fact]
        public async Task GetGame_WhenGameExists_ReturnsGameDto()
        {
            var gameId = 1;
            var expectedGame = new Game(4, 3, 10)
            {
                Id = gameId,
                BoardState = "X-O-X-----------",
                CurrentPlayer = Player.X,
                Status = GameStatus.InProgress
            };

            _gameServiceMock
                .Setup(x => x.GetGameAsync(gameId))
                .ReturnsAsync(expectedGame);

            var response = await _client.GetAsync($"/api/games/{gameId}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var gameDto = JsonConvert.DeserializeObject<GameDto>(content);

            Assert.Equal(expectedGame.BoardState, gameDto.BoardState);
            Assert.Equal(expectedGame.CurrentPlayer, gameDto.CurrentPlayer);
        }

        [Fact]
        public async Task GetGame_WhenGameNotFound_Returns404()
        {
            _gameServiceMock.Setup(x => x.GetGameAsync(It.IsAny<int>())).ReturnsAsync((Game)null);

            var response = await _client.GetAsync("/api/games/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostGame_CreatesNewGame_ReturnsCreatedGameDto()
        {
            var expectedGame = new Game(4, 3, 10)
            {
                BoardState = "X-O-X-----------",
                CurrentPlayer = Player.X,
                Status = GameStatus.InProgress
            };

            _gameServiceMock
                .Setup(x => x.CreateGameAsync())
                .ReturnsAsync(expectedGame);

            var response = await _client.PostAsync("/api/games", null);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var createdGameDto = JsonConvert.DeserializeObject<CreatedGameDto>(content);

            Assert.Equal(expectedGame.Id, createdGameDto.Id);
            Assert.Equal(expectedGame.BoardSize, createdGameDto.BoardSize);
            Assert.Equal($"/api/games/{expectedGame.Id}", response.Headers.Location?.ToString().ToLower());
        }
    }
}