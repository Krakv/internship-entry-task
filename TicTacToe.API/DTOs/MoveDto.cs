using TicTacToe.API.Models;

namespace TicTacToe.API.DTOs
{
    public class MoveDto
    {
        public required Player Player { get; set; }
        public required int Row { get; set; }
        public required int Col { get; set; }
    }
}
