using TicTacToe.API.Models;

namespace TicTacToe.API.DTOs
{
    /// <summary>
    /// DTO с полной информацией об игре
    /// </summary>
    public class GameDto
    {
        /// <summary>
        /// Текущее состояние игрового поля
        /// </summary>
        public string BoardState { get; set; }

        /// <summary>
        /// Условие победы
        /// </summary>
        public int WinnerLineLength { get; set; }

        /// <summary>
        /// Игрок, чей ход текущий
        /// </summary>
        public Player CurrentPlayer { get; set; }

        /// <summary>
        /// Текущий статус игры
        /// </summary>
        public GameStatus Status { get; set; }
    }
}
