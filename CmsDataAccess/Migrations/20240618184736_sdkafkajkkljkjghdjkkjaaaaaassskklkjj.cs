using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sdkafkajkkljkjghdjkkjaaaaaassskklkjj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ears",
                table: "PatientVisit",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ears",
                table: "PatientVisit");
        }
    }
}
