using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandBook.Web.Migrations
{
    /// <inheritdoc />
    public partial class addingImagesIncloudinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "image",
                table: "Posts",
                newName: "ImageLink");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageLink",
                table: "Posts",
                newName: "image");
        }
    }
}
