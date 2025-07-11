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

        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }
    }
}
