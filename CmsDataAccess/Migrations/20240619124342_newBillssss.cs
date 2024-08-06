using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class newBillssss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CenterMedicineListId",
                table: "VisitMedicine");

            migrationBuilder.AlterColumn<string>(
                name: "MedicineUnit",
                table: "VisitMedicine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "VisitMedicine",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "VisitMedicine",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MedicineName",
                table: "VisitMedicine",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "PricePerUnit",
                table: "CenterMedicineUnit",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "VisistBill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceCost = table.Column<double>(type: "float", nullable: true),
                    MedicnieCost = table.Column<double>(type: "float", nullable: true),
                    PaymentType = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CenterServicesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisistBill", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisistBill");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "VisitMedicine");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "VisitMedicine");

            migrationBuilder.DropColumn(
                name: "MedicineName",
                table: "VisitMedicine");

            migrationBuilder.AlterColumn<double>(
                name: "MedicineUnit",
                table: "VisitMedicine",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "CenterMedicineListId",
                table: "VisitMedicine",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "PricePerUnit",
                table: "CenterMedicineUnit",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
