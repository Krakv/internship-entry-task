using TicTacToe.API.Models;

namespace TicTacToe.API.DTOs
{
    public class CreatedGameDto
    {
        public int Id { get; set; }
        public int BoardSize { get; set; }
        public GameStatus Status { get; set; }
    }
}
