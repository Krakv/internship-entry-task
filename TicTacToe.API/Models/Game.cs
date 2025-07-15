namespace TicTacToe.API.Models
{
    /// <summary>
    /// Представляет игру в крестики-нолики
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Уникальный идентификатор игры
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Размер игрового поля (N x N)
        /// </summary>
        public int BoardSize { get; set; }

        /// <summary>
        /// Текущее состояние поля в виде строки, где '-' - пустая клетка
        /// </summary>
        public string BoardState { get; set; }

        /// <summary>
        /// Игрок, чей ход текущий (по умолчанию X)
        /// </summary>
        public Player CurrentPlayer { get; set; } = Player.X;

        /// <summary>
        /// Текущий статус игры (по умолчанию InProgress)
        /// </summary>
        public GameStatus Status { get; set; } = GameStatus.InProgress;

        /// <summary>
        /// Количество одинаковых символов подряд для победы
        /// </summary>
        public int WinnerLineLength { get; set; }

        /// <summary>
        /// Вероятность случайной смены символа (0-100%)
        /// </summary>
        public int SignChangeChance { get; set; }

        /// <summary>
        /// Список сделанных ходов в этой игре
        /// </summary>
        List<Move> Moves { get; set; } = new();

        /// <summary>
        /// Создает новую игру с указанными параметрами
        /// </summary>
        /// <param name="boardSize">Размер игрового поля</param>
        /// <param name="winnerLineLength">Количество символов для победы</param>
        /// <param name="signChangeChance">Шанс случайной смены символа (0-100)</param>
        public Game(int boardSize, int winnerLineLength, int signChangeChance)
        {
            BoardSize = boardSize;
            BoardState = new string('-', boardSize * boardSize);
            WinnerLineLength = winnerLineLength;
            SignChangeChance = signChangeChance;
        }
    }
}
