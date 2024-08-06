using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updatAppointmnet12545648548 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "PatientVisit",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "Appointment",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "PatientVisit");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Appointment");
        }
    }
}
