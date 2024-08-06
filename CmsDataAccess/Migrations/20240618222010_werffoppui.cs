using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class werffoppui : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerUnit",
                table: "CenterMedicineList");

            migrationBuilder.DropColumn(
                name: "SmallestUnit",
                table: "CenterMedicineList");

            migrationBuilder.CreateTable(
                name: "CenterMedicineUnit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterMedicineListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SmallestUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterMedicineUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterMedicineUnit_CenterMedicineList_CenterMedicineListId",
                        column: x => x.CenterMedicineListId,
                        principalTable: "CenterMedicineList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitMedicine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterMedicineListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicineUnit = table.Column<double>(type: "float", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitMedicine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitMedicine_PatientVisit_PatientVisitId",
                        column: x => x.PatientVisitId,
                        principalTable: "PatientVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CenterMedicineUnit_CenterMedicineListId",
                table: "CenterMedicineUnit",
                column: "CenterMedicineListId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitMedicine_PatientVisitId",
                table: "VisitMedicine",
                column: "PatientVisitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CenterMedicineUnit");

            migrationBuilder.DropTable(
                name: "VisitMedicine");

            migrationBuilder.AddColumn<string>(
                name: "PricePerUnit",
                table: "CenterMedicineList",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SmallestUnit",
                table: "CenterMedicineList",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
