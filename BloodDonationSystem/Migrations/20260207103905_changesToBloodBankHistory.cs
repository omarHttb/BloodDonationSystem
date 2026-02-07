using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonationSystem.Migrations
{
    /// <inheritdoc />
    public partial class changesToBloodBankHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodAddDate",
                table: "BloodBankHistory");

            migrationBuilder.DropColumn(
                name: "QuantityAddedToBloodBank",
                table: "BloodBankHistory");

            migrationBuilder.DropColumn(
                name: "QuantityTakenFromBloodBank",
                table: "BloodBankHistory");

            migrationBuilder.RenameColumn(
                name: "BloodTakeDate",
                table: "BloodBankHistory",
                newName: "TransactionDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsBloodAdded",
                table: "BloodBankHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "QuantityTransaction",
                table: "BloodBankHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBloodAdded",
                table: "BloodBankHistory");

            migrationBuilder.DropColumn(
                name: "QuantityTransaction",
                table: "BloodBankHistory");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "BloodBankHistory",
                newName: "BloodTakeDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "BloodAddDate",
                table: "BloodBankHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "QuantityAddedToBloodBank",
                table: "BloodBankHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantityTakenFromBloodBank",
                table: "BloodBankHistory",
                type: "int",
                nullable: true);
        }
    }
}
