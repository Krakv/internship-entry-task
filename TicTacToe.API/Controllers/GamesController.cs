using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.Models;
using TicTacToe.API.Repositories;

namespace TicTacToe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GameFactory _gameFactory;

        public GamesController(AppDbContext context, GameFactory gameFactory)
        {
            _context = context;
            _gameFactory = gameFactory;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            try
            {
                var game = await _context.Games.FindAsync(id);

                if (game == null) return NotFound();
                else return Ok(game);
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
                var game = _gameFactory.CreateGame();

                await _context.Games.AddAsync(game);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetGame), new { game.Id }, null);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
