using System.Linq;
using ContentWatcherBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentWatcherBot.Migrations
{
    public partial class UpdateMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdateMessage",
                table: "GuildWatchers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateMessage",
                table: "GuildWatchers");
        }
    }
}