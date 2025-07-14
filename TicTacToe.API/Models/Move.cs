namespace TicTacToe.API.Models
{
    public class Move
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public Player Player { get; set; }
        public DateTime MoveTime { get; set; } = DateTime.UtcNow;
    }
}
