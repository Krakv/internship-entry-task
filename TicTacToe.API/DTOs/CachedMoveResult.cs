namespace TicTacToe.API.DTOs
{
    public class CachedMoveResult
    {
        public CreatedMoveDto Response { get; set; }
        public string ETag { get; set; }
    }
}
