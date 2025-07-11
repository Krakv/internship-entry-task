using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    public class GameFactory
    {
        private readonly int _boardSize;

        public GameFactory(int boardSize)
        {
            _boardSize = boardSize;
        }

        public Game CreateGame()
        {
            return new Game(_boardSize);
        }
    }
}
