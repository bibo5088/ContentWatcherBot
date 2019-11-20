using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ContentWatcherBot.Database.Watchers;
using Microsoft.EntityFrameworkCore;
using ChannelMessages = System.Collections.Generic.IDictionary<ulong, System.Collections.Generic.IEnumerable<string>>;

namespace ContentWatcherBot.Database
{
    public class WatcherContext : DbContext
    {
        public DbSet<Watcher> Watchers { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Hook> Hooks { get; set; }

        public WatcherContext()
        {
        }

        public WatcherContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            var server = Environment.GetEnvironmentVariable("DB_SERVER");
            var name = Environment.GetEnvironmentVariable("DB_NAME");
            var user = Environment.GetEnvironmentVariable("DB_USER");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            optionsBuilder.UseMySql($"Server={server};Database={name};Uid={user};Pwd={password};");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Watcher>()
                .Property(w => w.PreviousContentIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions {IgnoreNullValues = true}),
                    v => JsonSerializer.Deserialize<HashSet<string>>(v,
                        new JsonSerializerOptions {IgnoreNullValues = true}));

            //Inheritance
            modelBuilder.Entity<Watcher>().HasDiscriminator<WatcherType>(nameof(Watcher.Type))
                .HasValue<MangadexWatcher>(WatcherType.Mangadex)
                .HasValue<ItchIoWatcher>(WatcherType.ItchIo)
                .HasValue<RssFeedWatcher>(WatcherType.RssFeed);

            modelBuilder.Entity<MangadexWatcher>().HasBaseType<Watcher>();
            modelBuilder.Entity<ItchIoWatcher>().HasBaseType<Watcher>();
            modelBuilder.Entity<RssFeedWatcher>().HasBaseType<Watcher>();
        }

        public async Task<Watcher> AddWatcher(Watcher watcher)
        {
            return await Watchers.SingleOrCreateAsync(w => w.HashCode == watcher.GetHashCode(), async () =>
            {
                await watcher.FirstFetch();
                await Watchers.AddAsync(watcher);
                await SaveChangesAsync();

                return watcher;
            });
        }

        public async Task<Guild> AddGuild(ulong discordId)
        {
            return await Guilds.SingleOrCreateAsync(s => s.GuildId == discordId, async () =>
            {
                var guild = new Guild {GuildId = discordId};
                await Guilds.AddAsync(guild);
                await SaveChangesAsync();

                return guild;
            });
        }

        public async Task<Hook> AddHook(Guild guild, Watcher watcher, ulong channelId,
            string updateMessage = null)
        {
            return await Hooks.SingleOrCreateAsync(sw =>
                sw.GuildId == guild.Id && sw.WatcherId == watcher.Id && sw.ChannelId == channelId, async () =>
            {
                //Create new serverWatcher
                var guildWatcher = new Hook
                {
                    Guild = guild, Watcher = watcher,
                    ChannelId = channelId,
                    UpdateMessage = updateMessage ?? $"New content from \"{watcher.Title}\""
                };
                await Hooks.AddAsync(guildWatcher);
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
                    var content = (await watcher.NewContent()).ToArray();

                    foreach (var guildWatcher in watcher.Hooks)
                    {
                        //Add UpdateMessage
                        var contentWithMessages = content.Select(c => $"{guildWatcher.UpdateMessage}\n{c}").ToList();

                        result.AddOrUpdate(guildWatcher.ChannelId, contentWithMessages, (key, list) =>
                        {
                            list.AddRange(contentWithMessages);
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
            var tasks = Watchers.Include(w => w.Hooks).Where(w => w.Hooks.Any()).AsEnumerable().Select(watcher => process(watcher));

            await Task.WhenAll(tasks);

            //Save previousIds
            await SaveChangesAsync();

            return result;
        }

        public async Task UpdateWatchers()
        {
            var newContentIgnoreExceptions = new Func<Watcher, Task>(async (watcher) =>
            {
                try
                {
                    await watcher.NewContent();
                }
                catch
                {
                    //Ignore
                }
            });

            var tasks = Watchers.AsEnumerable().Select(watcher => newContentIgnoreExceptions(watcher));
            await Task.WhenAll(tasks);

            //Save previousIds
            await SaveChangesAsync();
        }
    }
}