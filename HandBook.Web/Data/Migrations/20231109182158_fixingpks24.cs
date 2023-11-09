using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandBook.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixingpks24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LikeId",
                table: "Likes",
                type: "int",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
