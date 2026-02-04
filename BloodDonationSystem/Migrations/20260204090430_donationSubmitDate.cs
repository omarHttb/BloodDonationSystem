using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonationSystem.Migrations
{
    /// <inheritdoc />
    public partial class donationSubmitDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donors_BloodType_BloodTypeId",
                table: "Donors");

            migrationBuilder.AlterColumn<int>(
                name: "BloodTypeId",
                table: "Donors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "DonationSubmitDate",
                table: "Donations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Donors_BloodType_BloodTypeId",
                table: "Donors",
                column: "BloodTypeId",
                principalTable: "BloodType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donors_BloodType_BloodTypeId",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "DonationSubmitDate",
                table: "Donations");

            migrationBuilder.AlterColumn<int>(
                name: "BloodTypeId",
                table: "Donors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Donors_BloodType_BloodTypeId",
                table: "Donors",
                column: "BloodTypeId",
                principalTable: "BloodType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
