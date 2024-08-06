using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class upappddddkkjdddlklkkl54lll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "CenterServices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "COrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecipientAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientTelephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeliveryCost = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    COrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemPrice = table.Column<double>(type: "float", nullable: false),
                    ItemQuantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_COrderItems_COrder_COrderId",
                        column: x => x.COrderId,
                        principalTable: "COrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_COrderItems_COrderId",
                table: "COrderItems",
                column: "COrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "COrderItems");

            migrationBuilder.DropTable(
                name: "COrder");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "CenterServices");
        }
    }
}
