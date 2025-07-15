using Microsoft.EntityFrameworkCore;
using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    /// <summary>
    /// Контекст базы данных для приложения "Крестики-нолики"
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Инициализирует новый экземпляр контекста базы данных
        /// </summary>
        /// <param name="options">Параметры конфигурации для DbContext</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Набор данных для игр
        /// </summary>
        public virtual DbSet<Game> Games { get; set; }

        /// <summary>
        /// Набор данных для ходов
        /// </summary>
        public virtual DbSet<Move> Moves { get; set; }
    }
}
