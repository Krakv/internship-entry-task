namespace TicTacToe.API.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int BoardSize { get; set; }
        public string BoardState { get; set; }
        public Player CurrentPlayer { get; set; } = Player.X;
        public GameStatus Status { get; set; } = GameStatus.InProgress;
        public int WinnerLineLength { get; set; }
        public int SignChangeChance { get; set; }

        List<Move> Moves { get; set; } = new();

        public Game(int boardSize, int winnerLineLength, int signChangeChance)
        {
            BoardSize = boardSize;
            BoardState = new string('-', boardSize * boardSize);
            WinnerLineLength = winnerLineLength;
            SignChangeChance = signChangeChance;
        }
    }
}
