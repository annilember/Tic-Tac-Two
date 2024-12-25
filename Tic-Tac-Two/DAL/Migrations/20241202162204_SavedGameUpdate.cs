using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SavedGameUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModeName",
                table: "SavedGames",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlayerOName",
                table: "SavedGames",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlayerOPassword",
                table: "SavedGames",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlayerXName",
                table: "SavedGames",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlayerXPassword",
                table: "SavedGames",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeName",
                table: "SavedGames");

            migrationBuilder.DropColumn(
                name: "PlayerOName",
                table: "SavedGames");

            migrationBuilder.DropColumn(
                name: "PlayerOPassword",
                table: "SavedGames");

            migrationBuilder.DropColumn(
                name: "PlayerXName",
                table: "SavedGames");

            migrationBuilder.DropColumn(
                name: "PlayerXPassword",
                table: "SavedGames");
        }
    }
}
