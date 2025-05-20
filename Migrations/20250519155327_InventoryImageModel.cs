using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS_App.Migrations
{
    /// <inheritdoc />
    public partial class InventoryImageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductInventoryId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrlPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryImage_Product Category_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "Product Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryImage_ProductInventories_ProductInventoryId",
                        column: x => x.ProductInventoryId,
                        principalTable: "ProductInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryImage_ProductCategoryId",
                table: "InventoryImage",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryImage_ProductInventoryId",
                table: "InventoryImage",
                column: "ProductInventoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryImage");
        }
    }
}
