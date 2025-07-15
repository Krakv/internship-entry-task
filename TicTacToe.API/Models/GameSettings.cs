namespace TicTacToe.API.Models
{
    /// <summary>
    /// Настройки для создания новой игры
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// Размер игрового поля (по умолчанию 3)
        /// </summary>
        public int BoardSize { get; set; } = 3;

        /// <summary>
        /// Количество одинаковых символов подряд для победы (по умолчанию 3)
        /// </summary>
        public int WinnerLineLength { get; set; } = 3;

        /// <summary>
        /// Вероятность случайной смены символа (0-100%, по умолчанию 10)
        /// </summary>
        public int SignChangeChance { get; set; } = 10; // %
    }
}
