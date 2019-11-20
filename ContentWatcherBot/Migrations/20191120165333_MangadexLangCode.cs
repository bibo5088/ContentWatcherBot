using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentWatcherBot.Migrations
{
    public partial class MangadexLangCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lang",
                table: "Watchers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lang",
                table: "Watchers");
        }
    }
}
