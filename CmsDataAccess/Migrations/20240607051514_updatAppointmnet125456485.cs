using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updatAppointmnet125456485 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientTherapyGoals_Pet_PetId",
                table: "PatientTherapyGoals");

            migrationBuilder.DropColumn(
                name: "PetOwnerId",
                table: "PatientTherapyGoals");

            migrationBuilder.AlterColumn<Guid>(
                name: "PetId",
                table: "PatientTherapyGoals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTherapyGoals_Pet_PetId",
                table: "PatientTherapyGoals",
                column: "PetId",
                principalTable: "Pet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientTherapyGoals_Pet_PetId",
                table: "PatientTherapyGoals");

            migrationBuilder.AlterColumn<Guid>(
                name: "PetId",
                table: "PatientTherapyGoals",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "PetOwnerId",
                table: "PatientTherapyGoals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTherapyGoals_Pet_PetId",
                table: "PatientTherapyGoals",
                column: "PetId",
                principalTable: "Pet",
                principalColumn: "Id");
        }
    }
}
