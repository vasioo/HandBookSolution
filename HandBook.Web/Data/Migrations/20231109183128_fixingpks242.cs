using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandBook.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixingpks242 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LikeId",
                table: "Likes",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Likes",
                newName: "LikeId");
        }
    }
}
