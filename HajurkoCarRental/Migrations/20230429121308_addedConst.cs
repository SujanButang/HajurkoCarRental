using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class addedConst : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Damages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Damages");
        }
    }
}
