using Microsoft.Extensions.Options;
using TicTacToe.API.Models;

namespace TicTacToe.API.Services
{
    /// <summary>
    /// Фабрика для создания игр с настройками
    /// </summary>
    public class GameFactory
    {
        private readonly GameSettings _settings;

        /// <summary>
        /// Инициализирует фабрику игр
        /// </summary>
        /// <param name="settings">Настройки игры</param>
        public GameFactory(IOptions<GameSettings> settings)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Создает новую игру с текущими настройками
        /// </summary>
        /// <returns>Новый экземпляр игры</returns>
        public virtual Game CreateGame()
        {
            return new Game(_settings.BoardSize, _settings.WinnerLineLength, _settings.SignChangeChance);
        }
    }
}
