using Microsoft.EntityFrameworkCore;
using Moq;
using TicTacToe.API.DTOs;
using TicTacToe.API.Models;
using TicTacToe.API.Repositories;

namespace TicTacToe.API.Services
{
    public class MoveServiceTests
    {
        private readonly Mock<IGameRepository> _mockGameRepository;
        private readonly Mock<IMoveRepository> _mockMoveRepository;
        private readonly MoveService _moveService;
        private readonly Mock<ICacheService> _mockCacheService;

        public MoveServiceTests()
        {
            _mockGameRepository = new Mock<IGameRepository>();
            _mockMoveRepository = new Mock<IMoveRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _moveService = new MoveService(_mockGameRepository.Object, _mockMoveRepository.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task MakeMoveAsync_WithValidMove_UpdatesGameState()
        {
            
            var gameId = 1;
            var initialBoard = "---------";
            var game = new Game(3, 3, 0)
            {
                Id = gameId,
                BoardState = initialBoard,
                CurrentPlayer = Player.X,
                Status = GameStatus.InProgress
            };

            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = 1,
                Col = 1
            };

            _mockGameRepository.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(game);
            _mockMoveRepository.Setup(r => r.GetMovesByGameIdAsync(gameId))
                .ReturnsAsync(new List<Move>());
            _mockMoveRepository.Setup(r => r.AddAsync(It.IsAny<Move>()))
                .ReturnsAsync((Move m) => new Move
                {
                    Id = m.Id,
                    Player = Player.X,
                    Row = 1,
                    Col = 1,
                    GameId = m.GameId,
                    MoveTime = m.MoveTime
                });

            var result = await _moveService.MakeMoveAsync(gameId, moveDto);

            Assert.Equal("----X----", result.BoardState);
            Assert.Equal(Player.O, result.CurrentPlayer);
            Assert.Equal(GameStatus.InProgress, result.Status);
            _mockGameRepository.Verify(r => r.UpdateAsync(game), Times.Once);
            _mockMoveRepository.Verify(r => r.AddAsync(It.IsAny<Move>()), Times.Once);
        }

        [Fact]
        public async Task MakeMoveAsync_WithWinningMove_UpdatesGameStatus()
        {
            
            var gameId = 1;
            var initialBoard = "XX-------";
            var game = new Game(3, 3, 0)
            {
                Id = gameId,
                BoardState = initialBoard,
                CurrentPlayer = Player.X,
                Status = GameStatus.InProgress
            };

            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = 0,
                Col = 2
            };

            _mockGameRepository.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(game);
            _mockMoveRepository.Setup(r => r.GetMovesByGameIdAsync(gameId))
                .ReturnsAsync(new List<Move>());
            _mockMoveRepository.Setup(r => r.AddAsync(It.IsAny<Move>()))
                .ReturnsAsync((Move m) => new Move
                {
                    Id = m.Id,
                    Player = Player.X,
                    Row = 0,
                    Col = 2,
                    GameId = m.GameId,
                    MoveTime = m.MoveTime
                });


            var result = await _moveService.MakeMoveAsync(gameId, moveDto);

            
            Assert.Equal("XXX------", result.BoardState);
            Assert.Equal(GameStatus.XWon, result.Status);
        }

        [Fact]
        public async Task MakeMoveAsync_WithDrawMove_UpdatesGameStatus()
        {
            var gameId = 1;
            var initialBoard = "XXOOOXXO-";
            var game = new Game(3, 3, 0)
            {
                Id = gameId,
                BoardState = initialBoard,
                CurrentPlayer = Player.X,
                Status = GameStatus.InProgress
            };

            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = 2,
                Col = 2
            };

            _mockGameRepository.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(game);
            _mockMoveRepository.Setup(r => r.GetMovesByGameIdAsync(gameId))
                .ReturnsAsync(new List<Move>());
            _mockMoveRepository.Setup(r => r.AddAsync(It.IsAny<Move>()))
                .ReturnsAsync((Move m) => new Move
                {
                    Id = m.Id,
                    Player = Player.X,
                    Row = 2,
                    Col = 2,
                    GameId = m.GameId,
                    MoveTime = m.MoveTime
                });

            var result = await _moveService.MakeMoveAsync(gameId, moveDto);
            
            Assert.Equal("XXOOOXXOX", result.BoardState);
            Assert.Equal(GameStatus.Draw, result.Status);
        }

        [Fact]
        public async Task MakeMoveAsync_WithSignChangeChance_SwitchesPlayer()
        {
            var gameId = 1;
            var initialBoard = "---------";
            var game = new Game(3, 3, 100)
            {
                Id = gameId,
                BoardState = initialBoard,
                CurrentPlayer = Player.X,
                Status = GameStatus.InProgress
            };

            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = 1,
                Col = 1
            };

            _mockGameRepository.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(game);
            _mockMoveRepository.Setup(r => r.GetMovesByGameIdAsync(gameId))
                .ReturnsAsync(new List<Move> { new Move(), new Move() }); 
            _mockMoveRepository.Setup(r => r.AddAsync(It.IsAny<Move>()))
                .ReturnsAsync((Move m) => m);

            var result = await _moveService.MakeMoveAsync(gameId, moveDto);

            Assert.Equal("----O----", result.BoardState);
            Assert.Equal(Player.O, result.CurrentPlayer);
        }

        [Fact]
        public async Task MakeMoveAsync_WithNonExistingGame_ThrowsException()
        {
            var gameId = 999;
            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = 1,
                Col = 1
            };

            _mockGameRepository.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync((Game)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _moveService.MakeMoveAsync(gameId, moveDto));
        }

        [Fact]
        public async Task MakeMoveAsync_WithInvalidMove_ThrowsException()
        {
            var gameId = 1;
            var initialBoard = "X--------";
            var game = new Game(3, 3, 0)
            {
                Id = gameId,
                BoardState = initialBoard,
                CurrentPlayer = Player.O,
                Status = GameStatus.InProgress
            };

            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = 0,
                Col = 0
            };

            _mockGameRepository.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(game);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _moveService.MakeMoveAsync(gameId, moveDto));
            Assert.Contains("Wrong player", exception.Message);
        }

        [Theory]
        [InlineData(null, "Game not found")]
        [InlineData(GameStatus.XWon, "Game has been finished")]
        [InlineData(GameStatus.OWon, "Game has been finished")]
        [InlineData(GameStatus.Draw, "Game has been finished")]
        public void IsValidMove_WithFinishedGame_ReturnsFalse(GameStatus? status, string expectedMessage)
        {
            Game? game = null;
            if (status != null)
            {
                game = new Game(3, 3, 0)
                {
                    Status = status.Value,
                    CurrentPlayer = Player.X,
                    BoardSize = 3
                };
            }

            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = 1,
                Col = 1
            };

            var isValid = _moveService.IsValidMove(game, moveDto, out var message);

            Assert.False(isValid);
            Assert.Equal(expectedMessage, message);
        }

        [Fact]
        public void IsValidMove_WithWrongPlayer_ReturnsFalse()
        {
            var game = new Game(3, 3, 0)
            {
                Status = GameStatus.InProgress,
                CurrentPlayer = Player.X,
                BoardSize = 3
            };

            var moveDto = new MoveDto
            {
                Player = Player.O,
                Row = 1,
                Col = 1
            };

            var isValid = _moveService.IsValidMove(game, moveDto, out var message);

            Assert.False(isValid);
            Assert.Equal("Wrong player", message);
        }

        [Theory]
        [InlineData(-1, 1, "Incorrect row")]
        [InlineData(3, 1, "Incorrect row")]
        [InlineData(1, -1, "Incorrect column")]
        [InlineData(1, 3, "Incorrect column")]
        public void IsValidMove_WithInvalidCoordinates_ReturnsFalse(int row, int col, string expectedMessage)
        {
            var game = new Game(3, 3, 0)
            {
                Status = GameStatus.InProgress,
                CurrentPlayer = Player.X,
                BoardSize = 3
            };

            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = row,
                Col = col
            };

            var isValid = _moveService.IsValidMove(game, moveDto, out var message);

            Assert.False(isValid);
            Assert.Equal(expectedMessage, message);
        }

        [Fact]
        public void IsValidMove_WithOccupiedPosition_ReturnsFalse()
        {
            var game = new Game(3, 3, 0)
            {
                Status = GameStatus.InProgress,
                CurrentPlayer = Player.X,
                BoardSize = 3,
                BoardState = "X--------"
            };

            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = 0,
                Col = 0
            };

            var isValid = _moveService.IsValidMove(game, moveDto, out var message);

            Assert.False(isValid);
            Assert.Equal("Position has already been filled", message);
        }

        [Fact]
        public void IsValidMove_WithValidMove_ReturnsTrue()
        {
            var game = new Game(3, 3, 0)
            {
                Status = GameStatus.InProgress,
                CurrentPlayer = Player.X,
                BoardSize = 3,
                BoardState = "---------"
            };

            var moveDto = new MoveDto
            {
                Player = Player.X,
                Row = 1,
                Col = 1
            };

            var isValid = _moveService.IsValidMove(game, moveDto, out var message);

            Assert.True(isValid);
            Assert.Empty(message);
        }

        [Theory]
        [InlineData("XXX------", 3, 3, true)] 
        [InlineData("O--O--O--", 3, 3, true)] 
        [InlineData("X---X---X", 3, 3, true)] 
        [InlineData("--X-X-X--", 3, 3, true)]
        [InlineData("XX-------", 3, 3, false)] 
        [InlineData("XXOOOXXOX", 3, 3, false)] 
        [InlineData("XXXX-----", 3, 4, false)] 
        [InlineData("XXXX-----", 3, 5, false)] 
        public void CheckWinner_ReturnsCorrectResult(string board, int boardSize, int winLength, bool expected)
        {
            var result = _moveService.CheckWinner(board, boardSize, winLength);
            
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("XXXOOOXXX", true)]
        [InlineData("XXXOOOXX-", false)]
        [InlineData("---------", false)]
        public void IsBoardFull_ReturnsCorrectResult(string board, bool expected)
        {
            var result = _moveService.IsBoardFull(board);

            Assert.Equal(expected, result);
        }
    }
}