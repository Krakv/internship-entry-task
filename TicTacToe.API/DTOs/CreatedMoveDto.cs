using TicTacToe.API.Models;

namespace TicTacToe.API.DTOs
{
    public class CreatedMoveDto
    {
        public string NewBoardState { get; set; }
        public GameStatus Status { get; set; }
    }
}
