using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class upappddddkkjddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Favourite",
                newName: "SubProductId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Basket",
                newName: "SubProductId");

            migrationBuilder.AlterColumn<string>(
                name: "PetType",
                table: "Pet",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Breed",
                table: "Pet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LifeStyle",
                table: "Pet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Basket",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Breed",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "LifeStyle",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Basket");

            migrationBuilder.RenameColumn(
                name: "SubProductId",
                table: "Favourite",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "SubProductId",
                table: "Basket",
                newName: "ProductId");

            migrationBuilder.AlterColumn<string>(
                name: "PetType",
                table: "Pet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
