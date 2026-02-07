using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedBloodBankHistoryAndRemovedBloodTakenFromBloodBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodBank_BloodType_BloodTypeId",
                table: "BloodBank");

            migrationBuilder.DropTable(
                name: "BloodTakenFromBloodBank");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BloodBank",
                table: "BloodBank");

            migrationBuilder.RenameTable(
                name: "BloodBank",
                newName: "BloodBanks");

            migrationBuilder.RenameIndex(
                name: "IX_BloodBank_BloodTypeId",
                table: "BloodBanks",
                newName: "IX_BloodBanks_BloodTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BloodBanks",
                table: "BloodBanks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BloodBankHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodBankId = table.Column<int>(type: "int", nullable: false),
                    QuantityTakenFromBloodBank = table.Column<int>(type: "int", nullable: true),
                    QuantityAddedToBloodBank = table.Column<int>(type: "int", nullable: true),
                    BloodTakeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodAddDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodBankHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodBankHistory_BloodBanks_BloodBankId",
                        column: x => x.BloodBankId,
                        principalTable: "BloodBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloodBankHistory_BloodBankId",
                table: "BloodBankHistory",
                column: "BloodBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodBanks_BloodType_BloodTypeId",
                table: "BloodBanks",
                column: "BloodTypeId",
                principalTable: "BloodType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodBanks_BloodType_BloodTypeId",
                table: "BloodBanks");

            migrationBuilder.DropTable(
                name: "BloodBankHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BloodBanks",
                table: "BloodBanks");

            migrationBuilder.RenameTable(
                name: "BloodBanks",
                newName: "BloodBank");

            migrationBuilder.RenameIndex(
                name: "IX_BloodBanks_BloodTypeId",
                table: "BloodBank",
                newName: "IX_BloodBank_BloodTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BloodBank",
                table: "BloodBank",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BloodTakenFromBloodBank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodBankId = table.Column<int>(type: "int", nullable: false),
                    BloodTakeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_BloodTakenFromBloodBank_BloodBankId",
                table: "BloodTakenFromBloodBank",
                column: "BloodBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodBank_BloodType_BloodTypeId",
                table: "BloodBank",
                column: "BloodTypeId",
                principalTable: "BloodType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
