using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class catfffddddddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductTranslation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyWords",
                table: "ProductTranslation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductTranslation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "ProductTranslation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductCategoriesId",
                table: "Product",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SubProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubproductCharacteristics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubproductCharacteristics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubproductCharacteristics_SubProduct_SubProductId",
                        column: x => x.SubProductId,
                        principalTable: "SubProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubProductImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProductImage_SubProduct_SubProductId",
                        column: x => x.SubProductId,
                        principalTable: "SubProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubproductCharacteristicsTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubproductCharacteristicsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubproductCharacteristicsTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubproductCharacteristicsTranslation_SubproductCharacteristics_SubproductCharacteristicsId",
                        column: x => x.SubproductCharacteristicsId,
                        principalTable: "SubproductCharacteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubProduct_ProductId",
                table: "SubProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubproductCharacteristics_SubProductId",
                table: "SubproductCharacteristics",
                column: "SubProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubproductCharacteristicsTranslation_SubproductCharacteristicsId",
                table: "SubproductCharacteristicsTranslation",
                column: "SubproductCharacteristicsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProductImage_SubProductId",
                table: "SubProductImage",
                column: "SubProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubproductCharacteristicsTranslation");

            migrationBuilder.DropTable(
                name: "SubProductImage");

            migrationBuilder.DropTable(
                name: "SubproductCharacteristics");

            migrationBuilder.DropTable(
                name: "SubProduct");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductTranslation");

            migrationBuilder.DropColumn(
                name: "KeyWords",
                table: "ProductTranslation");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductTranslation");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "ProductTranslation");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductCategoriesId",
                table: "Product");
        }
    }
}
