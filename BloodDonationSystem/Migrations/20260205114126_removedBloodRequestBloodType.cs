using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonationSystem.Migrations
{
    /// <inheritdoc />
    public partial class removedBloodRequestBloodType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodRequestBloodTypes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DonationDate",
                table: "Donations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "BloodTypeId",
                table: "BloodRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BloodRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequests_BloodTypeId",
                table: "BloodRequests",
                column: "BloodTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodRequests_BloodType_BloodTypeId",
                table: "BloodRequests",
                column: "BloodTypeId",
                principalTable: "BloodType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodRequests_BloodType_BloodTypeId",
                table: "BloodRequests");

            migrationBuilder.DropIndex(
                name: "IX_BloodRequests_BloodTypeId",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "BloodTypeId",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BloodRequests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DonationDate",
                table: "Donations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BloodRequestBloodTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodRequestId = table.Column<int>(type: "int", nullable: false),
                    BloodTypeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodRequestBloodTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodRequestBloodTypes_BloodRequests_BloodRequestId",
                        column: x => x.BloodRequestId,
                        principalTable: "BloodRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BloodRequestBloodTypes_BloodType_BloodTypeId",
                        column: x => x.BloodTypeId,
                        principalTable: "BloodType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequestBloodTypes_BloodRequestId",
                table: "BloodRequestBloodTypes",
                column: "BloodRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequestBloodTypes_BloodTypeId",
                table: "BloodRequestBloodTypes",
                column: "BloodTypeId");
        }
    }
}
