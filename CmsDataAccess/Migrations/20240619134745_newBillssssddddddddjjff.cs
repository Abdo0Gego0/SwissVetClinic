using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class newBillssssddddddddjjff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "VisistBill",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "VisistBill");
        }
    }
}
