using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TicTacToe.API.DTOs;
using TicTacToe.API.Models;
using TicTacToe.API.Repositories;

namespace TicTacToe.API.Services
{
    public class MoveService : IMoveService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IMoveRepository _moveRepository;
        private readonly ICacheService _cacheService;
        private readonly Random _random = new();

        public MoveService(IGameRepository gameRepository, IMoveRepository moveRepository, ICacheService cacheService)
        {
            _gameRepository = gameRepository;
            _moveRepository = moveRepository;
            _cacheService = cacheService;
        }

        public async Task<CachedMoveResult?> GetCachedMoveResultAsync(int gameId, MoveDto moveDto)
        {
            var key = GenerateMoveCacheKey(gameId, moveDto);
            var cachedJson = await _cacheService.GetAsync<string>(key);

            if (string.IsNullOrEmpty(cachedJson)) return null;

            return JsonSerializer.Deserialize<CachedMoveResult>(cachedJson!);
        }

        public async Task CacheMoveResultAsync(int gameId, MoveDto moveDto, CreatedMoveDto response, string etag)
        {
            var key = GenerateMoveCacheKey(gameId, moveDto);

            var cacheObj = new CachedMoveResult
            {
                Response = response,
                ETag = etag
            };

            var json = JsonSerializer.Serialize(cacheObj);
            await _cacheService.SetAsync<string>(key, json, TimeSpan.FromHours(1));
        }

        private string GenerateMoveCacheKey(int gameId, MoveDto moveDto)
        {
            var raw = $"{gameId}:{moveDto.Player}:{moveDto.Row}:{moveDto.Col}";
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = sha.ComputeHash(bytes);
            return $"move:{Convert.ToBase64String(hash)}";
        }

        public async Task<Game> MakeMoveAsync(int gameId, MoveDto moveDto)
        {
            var game = await _gameRepository.GetByIdAsync(gameId) ?? throw new ArgumentException("Game not found");

            if (!IsValidMove(game, moveDto, out string message))
            {
                throw new InvalidOperationException(message);
            }

            game.CurrentPlayer = game.CurrentPlayer == Player.X ? Player.O : Player.X;

            var movesCount = await _moveRepository.GetMovesByGameIdAsync(gameId);
            if (movesCount.Count % 3 == 2 && _random.Next(0, 100) < game.SignChangeChance)
            {
                moveDto.Player = moveDto.Player == Player.X ? Player.O : Player.X;
            }

            var board = UpdateBoardState(game.BoardState, moveDto);
            game.BoardState = board;

            if (CheckWinner(board, game.BoardSize, game.WinnerLineLength))
            {
                game.Status = moveDto.Player == Player.X ? GameStatus.XWon : GameStatus.OWon;
            }
            else
            {
                game.Status = IsBoardFull(board) ? GameStatus.Draw : GameStatus.InProgress;
            }

            var move = new Move
            {
                GameId = gameId,
                Player = moveDto.Player,
                Row = moveDto.Row,
                Col = moveDto.Col
            };
            await _moveRepository.AddAsync(move);

            await _gameRepository.UpdateAsync(game);

            return game;
        }

        public bool IsValidMove(Game game, MoveDto moveDto, out string message)
        {
            if (game == null)
            {
                message = "Game not found";
                return false;
            }
            if (game.Status != GameStatus.InProgress)
            {
                message = "Game has been finished";
                return false;
            }
            if (game.CurrentPlayer != moveDto.Player)
            {
                message = "Wrong player";
                return false;
            }
            if (game.WinnerLineLength > game.BoardSize || game.WinnerLineLength < 2)
            {
                message = "Invalid winning line length";
                return false;
            }
            if (moveDto.Row < 0 || moveDto.Row >= game.BoardSize)
            {
                message = "Incorrect row";
                return false;
            }
            if (moveDto.Col < 0 || moveDto.Col >= game.BoardSize)
            {
                message = "Incorrect column";
                return false;
            }

            var board = game.BoardState;
            var position = moveDto.Row * game.BoardSize + moveDto.Col;
            
            var isCorrectPosition = position < board.Length;
            var isEmptyCell = board[position] == '-';
            if (!isCorrectPosition)
            {
                message = "Position is out of board";
                return false;
            }
            if (!isEmptyCell)
            {
                message = "Position has already been filled";
                return false;
            }
            message = "";
            return true;
        }

        private string UpdateBoardState(string currentState, MoveDto moveDto)
        {
            var boardSize = (int)Math.Sqrt(currentState.Length);
            var position = moveDto.Row * boardSize + moveDto.Col;
            var sb = new StringBuilder(currentState);
            sb[position] = moveDto.Player == Player.X ? 'X' : 'O';
            return sb.ToString();
        }

        public bool CheckWinner(string board, int boardSize, int winnerLineLength)
        {
            int size = boardSize;
            int winLength = winnerLineLength;
            char[] players = { 'X', 'O' };

            foreach (var player in players)
            {
                for (int row = 0; row < size; row++)
                {
                    for (int col = 0; col <= size - winLength; col++)
                    {
                        bool win = true;
                        for (int k = 0; k < winLength; k++)
                        {
                            if (board[row * size + col + k] != player)
                            {
                                win = false;
                                break;
                            }
                        }
                        if (win) return true;
                    }
                }

                for (int col = 0; col < size; col++)
                {
                    for (int row = 0; row <= size - winLength; row++)
                    {
                        bool win = true;
                        for (int k = 0; k < winLength; k++)
                        {
                            if (board[(row + k) * size + col] != player)
                            {
                                win = false;
                                break;
                            }
                        }
                        if (win) return true;
                    }
                }

                for (int row = 0; row <= size - winLength; row++)
                {
                    for (int col = 0; col <= size - winLength; col++)
                    {
                        bool win = true;
                        for (int k = 0; k < winLength; k++)
                        {
                            if (board[(row + k) * size + (col + k)] != player)
                            {
                                win = false;
                                break;
                            }
                        }
                        if (win) return true;
                    }
                }

                for (int row = winLength - 1; row < size; row++)
                {
                    for (int col = 0; col <= size - winLength; col++)
                    {
                        bool win = true;
                        for (int k = 0; k < winLength; k++)
                        {
                            if (board[(row - k) * size + (col + k)] != player)
                            {
                                win = false;
                                break;
                            }
                        }
                        if (win) return true;
                    }
                }
            }

            return false;
        }

        public bool IsBoardFull(string board)
        {
            return !board.Contains('-');
        }
    }
}
