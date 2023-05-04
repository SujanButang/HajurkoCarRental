using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class Damagesworkout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Damages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Damages");
        }
    }
}
