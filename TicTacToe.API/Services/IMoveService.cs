using TicTacToe.API.DTOs;
using TicTacToe.API.Models;

namespace TicTacToe.API.Services
{
    public interface IMoveService
    {
        Task<Game> MakeMoveAsync(int gameId, MoveDto moveDto);
        Task<CachedMoveResult?> GetCachedMoveResultAsync(int gameId, MoveDto moveDto);
        Task CacheMoveResultAsync(int gameId, MoveDto moveDto, CreatedMoveDto response, string etag);
        bool IsValidMove(Game game, MoveDto moveDto, out string message);
    }
}
