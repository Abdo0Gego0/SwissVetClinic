using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sdkafkajkkljkjghdjkkjaaaaaassskklk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vaccinated",
                table: "Pet");

            migrationBuilder.AddColumn<int>(
                name: "Vaccinated",
                table: "PatientVisit",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vaccinated",
                table: "PatientVisit");

            migrationBuilder.AddColumn<int>(
                name: "Vaccinated",
                table: "Pet",
                type: "int",
                nullable: true);
        }
    }
}
