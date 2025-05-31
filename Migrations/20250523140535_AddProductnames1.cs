using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS_App.Migrations
{
    /// <inheritdoc />
    public partial class AddProductnames1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProducts_ProductInventories_ProductInventoryId",
                table: "CategoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProducts_ProductNames_ProductNameId",
                table: "CategoryProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryProducts",
                table: "CategoryProducts");

            migrationBuilder.DropIndex(
                name: "IX_CategoryProducts_ProductNameId",
                table: "CategoryProducts");

            migrationBuilder.DropColumn(
                name: "ProductInventoryId",
                table: "CategoryProducts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductNameId",
                table: "CategoryProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryProducts",
                table: "CategoryProducts",
                columns: new[] { "ProductNameId", "ProductCategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProducts_ProductNames_ProductNameId",
                table: "CategoryProducts",
                column: "ProductNameId",
                principalTable: "ProductNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProducts_ProductNames_ProductNameId",
                table: "CategoryProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryProducts",
                table: "CategoryProducts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductNameId",
                table: "CategoryProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductInventoryId",
                table: "CategoryProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryProducts",
                table: "CategoryProducts",
                columns: new[] { "ProductInventoryId", "ProductCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProducts_ProductNameId",
                table: "CategoryProducts",
                column: "ProductNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProducts_ProductInventories_ProductInventoryId",
                table: "CategoryProducts",
                column: "ProductInventoryId",
                principalTable: "ProductInventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProducts_ProductNames_ProductNameId",
                table: "CategoryProducts",
                column: "ProductNameId",
                principalTable: "ProductNames",
                principalColumn: "Id");
        }
    }
}
