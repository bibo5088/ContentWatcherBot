using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContentWatcherBot.Database
{
    public class WatcherContext : DbContext
    {
        public DbSet<Watcher> Watchers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }

        public async Task AddWatcher(Watcher watcher)
        {
            if (!Watchers.Contains(watcher))
            {
                await watcher.FirstFetch();
                Watchers.Add(watcher);

                await SaveChangesAsync();
            }
        }
    }
}