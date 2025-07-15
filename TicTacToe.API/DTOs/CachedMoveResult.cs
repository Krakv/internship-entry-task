namespace TicTacToe.API.DTOs
{
    /// <summary>
    /// Результат кэшированного хода
    /// </summary>
    public class CachedMoveResult
    {
        /// <summary>
        /// Ответ с данными о выполненном ходе
        /// </summary>
        public CreatedMoveDto Response { get; set; }

        /// <summary>
        /// ETag для валидации кэша
        /// </summary>
        public string ETag { get; set; }
    }
}
