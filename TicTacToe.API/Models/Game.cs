namespace TicTacToe.API.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int BoardSize { get; set; }
        public string BoardState { get; set; }
        public Player CurrentPlayer { get; set; } = Player.X;
        public GameStatus Status { get; set; } = GameStatus.InProgress;

        List<Move> Moves { get; set; } = new();

        public Game(int boardSize)
        {
            BoardSize = boardSize;
            BoardState = new string('-', boardSize * boardSize);
        }
    }
}
