using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.DTOs;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GamesController> _logger;

        public GamesController(IGameService gameService, ILogger<GamesController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            try
            {
                _logger.LogTrace("Начало операции получения данных об игре с ID:{GameId}", id);
                var game = await _gameService.GetGameAsync(id);
                if (game == null)
                {
                    _logger.LogWarning("Игра с ID:{GameId} не найдена.", id);
                    return NotFound();
                }
                _logger.LogTrace("Найдена игра с ID:{GameId}", id);
                return Ok(
                    new GameDto() { 
                        BoardState = game.BoardState, 
                        CurrentPlayer = game.CurrentPlayer, 
                        Status = game.Status
                    });
            }
            catch (Exception e)
            {
                _logger.LogError("Непредвиденная ошибка. {Exception}", e);
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostGame()
        {
            try
            {
                _logger.LogTrace("Начало операции создания игры.");
                var game = await _gameService.CreateGameAsync();
                _logger.LogTrace("Игра создана с ID:{GameId}.", game.Id);
                return Created($"/api/games/{game.Id}", 
                    new CreatedGameDto () 
                    { 
                        Id = game.Id, 
                        BoardSize = game.BoardSize, 
                        Status = game.Status
                    });
            }
            catch (Exception e)
            {
                _logger.LogError("Непредвиденная ошибка. {Exception}", e);
                return BadRequest(e.Message);
            }
        }
    }
}
