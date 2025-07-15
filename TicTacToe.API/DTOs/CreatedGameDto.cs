using TicTacToe.API.Models;

namespace TicTacToe.API.DTOs
{
    /// <summary>
    /// DTO с информацией о созданной игре
    /// </summary>
    public class CreatedGameDto
    {
        /// <summary>
        /// Идентификатор игры
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Размер игрового поля
        /// </summary>
        public int BoardSize { get; set; }

        /// <summary>
        /// Текущий статус игры
        /// </summary>
        public GameStatus Status { get; set; }
    }
}
