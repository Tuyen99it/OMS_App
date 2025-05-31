using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS_App.Migrations
{
    /// <inheritdoc />
    public partial class AddProductnames2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImages_ProductInventories_ProductInventoryId",
                table: "InventoryImages");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImages_ProductNames_ProductNameId",
                table: "InventoryImages");

            migrationBuilder.DropIndex(
                name: "IX_InventoryImages_ProductInventoryId",
                table: "InventoryImages");

            migrationBuilder.DropColumn(
                name: "ProductInventoryId",
                table: "InventoryImages");

            migrationBuilder.AlterColumn<int>(
                name: "ProductNameId",
                table: "InventoryImages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryImages_ProductNames_ProductNameId",
                table: "InventoryImages",
                column: "ProductNameId",
                principalTable: "ProductNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImages_ProductNames_ProductNameId",
                table: "InventoryImages");

            migrationBuilder.AlterColumn<int>(
                name: "ProductNameId",
                table: "InventoryImages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductInventoryId",
                table: "InventoryImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryImages_ProductInventoryId",
                table: "InventoryImages",
                column: "ProductInventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryImages_ProductInventories_ProductInventoryId",
                table: "InventoryImages",
                column: "ProductInventoryId",
                principalTable: "ProductInventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryImages_ProductNames_ProductNameId",
                table: "InventoryImages",
                column: "ProductNameId",
                principalTable: "ProductNames",
                principalColumn: "Id");
        }
    }
}
