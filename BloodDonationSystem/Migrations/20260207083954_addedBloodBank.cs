using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonationSystem.Migrations
{
    /// <inheritdoc />
    public partial class addedBloodBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BloodBank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodTypeId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodBank_BloodType_BloodTypeId",
                        column: x => x.BloodTypeId,
                        principalTable: "BloodType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BloodTakenFromBloodBank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodBankId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TakenDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodTakenFromBloodBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodTakenFromBloodBank_BloodBank_BloodBankId",
                        column: x => x.BloodBankId,
                        principalTable: "BloodBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloodBank_BloodTypeId",
                table: "BloodBank",
                column: "BloodTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodTakenFromBloodBank_BloodBankId",
                table: "BloodTakenFromBloodBank",
                column: "BloodBankId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodTakenFromBloodBank");

            migrationBuilder.DropTable(
                name: "BloodBank");
        }
    }
}
