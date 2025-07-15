using TicTacToe.API.Models;

namespace TicTacToe.API.DTOs
{
    /// <summary>
    /// DTO с информацией о совершаемом ходе
    /// </summary>
    public class MoveDto
    {
        /// <summary>
        /// Игрок, совершающий ход (X или O)
        /// </summary>
        public required Player Player { get; set; }

        /// <summary>
        /// Номер строки (начиная с 0)
        /// </summary>
        public required int Row { get; set; }

        /// <summary>
        /// Номер столбца (начиная с 0)
        /// </summary>
        public required int Col { get; set; }
    }
}
