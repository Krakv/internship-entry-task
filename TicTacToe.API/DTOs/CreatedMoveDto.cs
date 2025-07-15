using TicTacToe.API.Models;

namespace TicTacToe.API.DTOs
{
    /// <summary>
    /// DTO с информацией о выполненном ходе
    /// </summary>
    public class CreatedMoveDto
    {
        /// <summary>
        /// Новое состояние игрового поля после хода
        /// </summary>
        public string NewBoardState { get; set; }

        /// <summary>
        /// Обновленный статус игры после хода
        /// </summary>
        public GameStatus Status { get; set; }
    }
}
