using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updatAppointmnet1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CenterServicesId",
                table: "Appointment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PetId",
                table: "Appointment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CenterServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CenterServicesTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CenterServicesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterServicesTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterServicesTranslation_CenterServices_CenterServicesId",
                        column: x => x.CenterServicesId,
                        principalTable: "CenterServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CenterServicesTranslation_CenterServicesId",
                table: "CenterServicesTranslation",
                column: "CenterServicesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CenterServicesTranslation");

            migrationBuilder.DropTable(
                name: "CenterServices");

            migrationBuilder.DropColumn(
                name: "CenterServicesId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "Appointment");
        }
    }
}
