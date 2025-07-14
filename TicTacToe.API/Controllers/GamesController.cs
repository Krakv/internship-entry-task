using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.DTOs;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            try
            {
                var game = await _gameService.GetGameAsync(id);
                return game == null ? NotFound() : Ok(
                    new GameDto() { 
                        BoardState = game.BoardState, 
                        CurrentPlayer = game.CurrentPlayer, 
                        Status = game.Status
                    });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostGame()
        {
            try
            {
                var game = await _gameService.CreateGameAsync();
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
                return BadRequest(e.Message);
            }
        }
    }
}
