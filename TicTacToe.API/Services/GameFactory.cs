using Microsoft.Extensions.Options;
using TicTacToe.API.Models;

namespace TicTacToe.API.Services
{
    public class GameFactory
    {
        private readonly GameSettings _settings;

        public GameFactory(IOptions<GameSettings> settings)
        {
            _settings = settings.Value;
        }

        public virtual Game CreateGame()
        {
            return new Game(_settings.BoardSize, _settings.WinnerLineLength, _settings.SignChangeChance);
        }
    }
}
