namespace TicTacToe.API.Models
{
    public class GameSettings
    {
        public int BoardSize { get; set; } = 3;
        public int WinnerLineLength { get; set; } = 3;
        public int SignChangeChance { get; set; } = 10; // %
    }
}
