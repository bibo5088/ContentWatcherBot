using ContentWatcherBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentWatcherBot.Migrations
{
    public partial class FillUpdateMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            using var context = new WatcherContext();
            foreach (var gw in context.GuildWatchers.Include(gw => gw.Watcher))
            {
                gw.UpdateMessage = $"New content from \"{gw.Watcher.Title}\"";
            }

            context.SaveChanges();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
