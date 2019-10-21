using Microsoft.EntityFrameworkCore;

namespace ContentWatcherBot.Database
{
    public class WatcherContext : DbContext
    {
        public DbSet<Watcher> Watchers { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Watcher>()
                .HasIndex(w => w.Name)
                .IsUnique();
        }
    }
}