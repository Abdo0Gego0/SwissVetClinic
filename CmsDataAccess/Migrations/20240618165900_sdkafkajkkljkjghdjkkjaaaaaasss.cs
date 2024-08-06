using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sdkafkajkkljkjghdjkkjaaaaaasss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeartRate",
                table: "PatientVisit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MuscSkel",
                table: "PatientVisit",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "PatientVisit");

            migrationBuilder.DropColumn(
                name: "MuscSkel",
                table: "PatientVisit");
        }
    }
}
