using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ContentWatcherBot.Fetcher;
using Microsoft.EntityFrameworkCore;
using ChannelMessages = System.Collections.Generic.IDictionary<ulong, System.Collections.Generic.IEnumerable<string>>;

namespace ContentWatcherBot.Database
{
    public class WatcherContext : DbContext
    {
        public DbSet<Watcher> Watchers { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerWatcher> ServerWatchers { get; set; }

        public WatcherContext()
        {
        }

        public WatcherContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=database.sqlite;");
            }
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

        public async Task<Watcher> AddWatcher(Uri url)
        {
            var alreadyExistingWatcher = await Watchers.SingleOrDefaultAsync(w => w.Url == url);

            if (alreadyExistingWatcher != null) return alreadyExistingWatcher;

            //Create new watcher
            var watcher = await WatcherFactory.CreateWatcher(url);
            await Watchers.AddAsync(watcher);
            await SaveChangesAsync();

            return watcher;
        }


        public async Task<ConcurrentDictionary<ulong, List<string>>> GetNewContentMessages()
        {
            var result = new ConcurrentDictionary<ulong, List<string>>();

            var process = new Func<Watcher, Task>(async watcher =>
            {
                try
                {
                    var content = (await watcher.NewContent()).ToList();

                    foreach (var channel in watcher.ServerWatchers.Select(s => s.ChannelId))
                    {
                        result.AddOrUpdate(channel, content, (key, list) =>
                        {
                            list.AddRange(content);
                            return list;
                        });
                    }
                }
                catch (Exception e)
                {
                    //Ignore errors
                    Console.WriteLine(e);
                }
            });

            //Execute tasks
            var tasks = Watchers.AsEnumerable().Select(watcher => process(watcher));

            await Task.WhenAll(tasks);

            return result;
        }
    }
}