using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContentWatcherBot.Database
{
    public class WatcherContext : DbContext
    {
        public DbSet<Watcher> Watchers { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerWatcher> ServerWatchers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.sqlite;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Watcher>()
                .Property(w => w.PreviousContentIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions {IgnoreNullValues = true}),
                    v => JsonSerializer.Deserialize<HashSet<string>>(v,
                        new JsonSerializerOptions {IgnoreNullValues = true}));
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