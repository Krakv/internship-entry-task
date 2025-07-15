namespace TicTacToe.API.Models
{
    /// <summary>
    /// Представляет ход в игре
    /// </summary>
    public class Move
    {
        /// <summary>
        /// Уникальный идентификатор хода
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер строки (начиная с 0)
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Номер столбца (начиная с 0)
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// Идентификатор игры, к которой относится ход
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// Игра, к которой относится ход
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Игрок, сделавший ход (X или O)
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Время совершения хода (UTC)
        /// </summary>
        public DateTime MoveTime { get; set; } = DateTime.UtcNow;
    }
}
