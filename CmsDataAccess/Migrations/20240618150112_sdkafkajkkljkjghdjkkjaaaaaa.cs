using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sdkafkajkkljkjghdjkkjaaaaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sex",
                table: "Pet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Vaccinated",
                table: "Pet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Abdomen",
                table: "PatientVisit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Behavior",
                table: "PatientVisit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BodyCondition",
                table: "PatientVisit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Prognosis",
                table: "PatientVisit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SKIN",
                table: "PatientVisit",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sex",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "Vaccinated",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "Abdomen",
                table: "PatientVisit");

            migrationBuilder.DropColumn(
                name: "Behavior",
                table: "PatientVisit");

            migrationBuilder.DropColumn(
                name: "BodyCondition",
                table: "PatientVisit");

            migrationBuilder.DropColumn(
                name: "Prognosis",
                table: "PatientVisit");

            migrationBuilder.DropColumn(
                name: "SKIN",
                table: "PatientVisit");
        }
    }
}
