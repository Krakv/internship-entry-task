using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TicTacToe.API.DTOs;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers
{
    [Route("api/games/{gameId}/[controller]")]
    [ApiController]
    public class MovesController : ControllerBase
    {
        private readonly IMoveService _moveService;

        public MovesController(IMoveService moveService)
        {
            _moveService = moveService;
        }

        [HttpPost]
        public async Task<IActionResult> MakeMove(int gameId, [FromBody] MoveDto moveDto)
        {
            try
            {
                var existingResult = await _moveService.GetCachedMoveResultAsync(gameId, moveDto);
                if (existingResult != null)
                {
                    Response.Headers.ETag = $"\"{existingResult.ETag}\"";
                    return Ok(existingResult.Response);
                }

                var game = await _moveService.MakeMoveAsync(gameId, moveDto);

                if (game == null)
                    return NotFound("Game not found");

                if (game.Status != Models.GameStatus.InProgress)
                    return Conflict("Game has been finished");

                var etag = GenerateETag(game);

                var result = new CreatedMoveDto
                {
                    NewBoardState = game.BoardState,
                    Status = game.Status
                };

                await _moveService.CacheMoveResultAsync(gameId, moveDto, result, etag);

                Response.Headers.ETag = $"\"{etag}\"";
                return Ok(result);
            }
            catch (JsonException)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid JSON",
                    Status = 400,
                    Detail = "The JSON body is malformed or invalid"
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private string GenerateETag(Game game)
        {
            var json = JsonSerializer.Serialize(game.BoardState);
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hash);
        }

    }
}
