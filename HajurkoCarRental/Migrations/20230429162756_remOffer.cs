using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class remOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_Offers_OfferId",
                table: "Car");

            migrationBuilder.DropIndex(
                name: "IX_Car_OfferId",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "Car");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OfferId",
                table: "Car",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Car_OfferId",
                table: "Car",
                column: "OfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_Car_Offers_OfferId",
                table: "Car",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
