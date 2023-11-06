using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messenger.Data.Migrations
{
    public partial class newnewafacewdwdvxfb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "Messages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
