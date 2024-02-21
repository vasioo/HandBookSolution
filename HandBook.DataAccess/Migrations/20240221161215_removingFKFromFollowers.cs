using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class removingFKFromFollowers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowedUserId",
                table: "Followers");

            migrationBuilder.DropColumn(
                name: "FollowerUserId",
                table: "Followers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FollowedUserId",
                table: "Followers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FollowerUserId",
                table: "Followers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
