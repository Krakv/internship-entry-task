using Microsoft.EntityFrameworkCore;
using TicTacToe.API.Models;

namespace TicTacToe.API.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Move> Moves { get; set; }
    }
}
