using TicTacToe.API.Models;

namespace TicTacToe.API.DTOs
{
    public class GameDto
    {
        public string BoardState { get; set; }
        public Player CurrentPlayer { get; set; } 
        public GameStatus Status { get; set; }
    }
}
