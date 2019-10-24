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
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<GuildWatcher> GuildWatchers { get; set; }

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
            return await Watchers.SingleOrCreateAsync(w => w.Url == url, async () =>
            {
                var watcher = await WatcherFactory.CreateWatcher(url);
                await Watchers.AddAsync(watcher);
                await SaveChangesAsync();

                return watcher;
            });
        }

        public async Task<Guild> AddServer(ulong discordId)
        {
            return await Guilds.SingleOrCreateAsync(s => s.GuildId == discordId, async () =>
            {
                var guild = new Guild {GuildId = discordId};
                await Guilds.AddAsync(guild);
                await SaveChangesAsync();

                return guild;
            });
        }

        public async Task<GuildWatcher> AddServerWatcher(Guild guild, Watcher watcher, ulong channelId)
        {
            return await GuildWatchers.SingleOrCreateAsync(sw =>
                sw.GuildId == guild.Id && sw.WatcherId == watcher.Id && sw.ChannelId == channelId, async () =>
            {
                //Create new serverWatcher
                var guildWatcher = new GuildWatcher {Guild = guild, Watcher = watcher, ChannelId = channelId};
                await GuildWatchers.AddAsync(guildWatcher);
                await SaveChangesAsync();

                return guildWatcher;
            });
        }


        public async Task<ConcurrentDictionary<ulong, List<string>>> GetNewContentMessages()
        {
            var result = new ConcurrentDictionary<ulong, List<string>>();

            var process = new Func<Watcher, Task>(async watcher =>
            {
                try
                {
                    var content = (await watcher.NewContent()).ToList();

                    foreach (var channel in watcher.GuildWatchers.Select(s => s.ChannelId))
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
            var tasks = Watchers.Include(w => w.GuildWatchers).AsEnumerable().Select(watcher => process(watcher));

            await Task.WhenAll(tasks);

            return result;
        }
    }
}