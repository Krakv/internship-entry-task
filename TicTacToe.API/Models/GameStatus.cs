namespace TicTacToe.API.Models
{
    /// <summary>
    /// Статус игры
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// Игра в процессе
        /// </summary>
        InProgress,

        /// <summary>
        /// Победа крестиков
        /// </summary>
        XWon,

        /// <summary>
        /// Победа ноликов
        /// </summary>
        OWon,

        /// <summary>
        /// Ничья
        /// </summary>
        Draw
    }
}
