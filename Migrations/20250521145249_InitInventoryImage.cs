using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS_App.Migrations
{
    /// <inheritdoc />
    public partial class InitInventoryImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImage_Product Category_ProductCategoryId",
                table: "InventoryImage");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImage_ProductInventories_ProductInventoryId",
                table: "InventoryImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryImage",
                table: "InventoryImage");

            migrationBuilder.RenameTable(
                name: "InventoryImage",
                newName: "InventoryImages");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryImage_ProductInventoryId",
                table: "InventoryImages",
                newName: "IX_InventoryImages_ProductInventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryImage_ProductCategoryId",
                table: "InventoryImages",
                newName: "IX_InventoryImages_ProductCategoryId");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "InventoryImages",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryImages",
                table: "InventoryImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryImages_Product Category_ProductCategoryId",
                table: "InventoryImages",
                column: "ProductCategoryId",
                principalTable: "Product Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryImages_ProductInventories_ProductInventoryId",
                table: "InventoryImages",
                column: "ProductInventoryId",
                principalTable: "ProductInventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImages_Product Category_ProductCategoryId",
                table: "InventoryImages");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImages_ProductInventories_ProductInventoryId",
                table: "InventoryImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryImages",
                table: "InventoryImages");

            migrationBuilder.RenameTable(
                name: "InventoryImages",
                newName: "InventoryImage");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryImages_ProductInventoryId",
                table: "InventoryImage",
                newName: "IX_InventoryImage_ProductInventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryImages_ProductCategoryId",
                table: "InventoryImage",
                newName: "IX_InventoryImage_ProductCategoryId");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "InventoryImage",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryImage",
                table: "InventoryImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryImage_Product Category_ProductCategoryId",
                table: "InventoryImage",
                column: "ProductCategoryId",
                principalTable: "Product Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryImage_ProductInventories_ProductInventoryId",
                table: "InventoryImage",
                column: "ProductInventoryId",
                principalTable: "ProductInventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
