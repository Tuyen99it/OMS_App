using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS_App.Migrations
{
    /// <inheritdoc />
    public partial class AddProductnames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anh san pham_Phan lop san pham_ProductCategoryId",
                table: "Anh san pham");

            migrationBuilder.DropForeignKey(
                name: "FK_Anh san pham_ProductNames_ProductNameId",
                table: "Anh san pham");

            migrationBuilder.DropForeignKey(
                name: "FK_Anh san pham_San pham trong kho_ProductInventoryId",
                table: "Anh san pham");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProducts_Phan lop san pham_ProductCategoryId",
                table: "CategoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProducts_San pham trong kho_ProductInventoryId",
                table: "CategoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Phan lop san pham_Phan lop san pham_ParentCategoryId",
                table: "Phan lop san pham");

            migrationBuilder.DropForeignKey(
                name: "FK_San pham trong kho_ProductNames_ProductNameId",
                table: "San pham trong kho");

            migrationBuilder.DropPrimaryKey(
                name: "PK_San pham trong kho",
                table: "San pham trong kho");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Phan lop san pham",
                table: "Phan lop san pham");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Anh san pham",
                table: "Anh san pham");

            migrationBuilder.RenameTable(
                name: "San pham trong kho",
                newName: "ProductInventories");

            migrationBuilder.RenameTable(
                name: "Phan lop san pham",
                newName: "ProductCategories");

            migrationBuilder.RenameTable(
                name: "Anh san pham",
                newName: "InventoryImages");

            migrationBuilder.RenameIndex(
                name: "IX_San pham trong kho_ProductNameId",
                table: "ProductInventories",
                newName: "IX_ProductInventories_ProductNameId");

            migrationBuilder.RenameIndex(
                name: "IX_Phan lop san pham_ParentCategoryId",
                table: "ProductCategories",
                newName: "IX_ProductCategories_ParentCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Anh san pham_ProductNameId",
                table: "InventoryImages",
                newName: "IX_InventoryImages_ProductNameId");

            migrationBuilder.RenameIndex(
                name: "IX_Anh san pham_ProductInventoryId",
                table: "InventoryImages",
                newName: "IX_InventoryImages_ProductInventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Anh san pham_ProductCategoryId",
                table: "InventoryImages",
                newName: "IX_InventoryImages_ProductCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductInventories",
                table: "ProductInventories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryImages",
                table: "InventoryImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProducts_ProductCategories_ProductCategoryId",
                table: "CategoryProducts",
                column: "ProductCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProducts_ProductInventories_ProductInventoryId",
                table: "CategoryProducts",
                column: "ProductInventoryId",
                principalTable: "ProductInventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryImages_ProductCategories_ProductCategoryId",
                table: "InventoryImages",
                column: "ProductCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                table: "ProductCategories",
                column: "ParentCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInventories_ProductNames_ProductNameId",
                table: "ProductInventories",
                column: "ProductNameId",
                principalTable: "ProductNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProducts_ProductCategories_ProductCategoryId",
                table: "CategoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProducts_ProductInventories_ProductInventoryId",
                table: "CategoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImages_ProductCategories_ProductCategoryId",
                table: "InventoryImages");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImages_ProductInventories_ProductInventoryId",
                table: "InventoryImages");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryImages_ProductNames_ProductNameId",
                table: "InventoryImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductInventories_ProductNames_ProductNameId",
                table: "ProductInventories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductInventories",
                table: "ProductInventories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryImages",
                table: "InventoryImages");

            migrationBuilder.RenameTable(
                name: "ProductInventories",
                newName: "San pham trong kho");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                newName: "Phan lop san pham");

            migrationBuilder.RenameTable(
                name: "InventoryImages",
                newName: "Anh san pham");

            migrationBuilder.RenameIndex(
                name: "IX_ProductInventories_ProductNameId",
                table: "San pham trong kho",
                newName: "IX_San pham trong kho_ProductNameId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_ParentCategoryId",
                table: "Phan lop san pham",
                newName: "IX_Phan lop san pham_ParentCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryImages_ProductNameId",
                table: "Anh san pham",
                newName: "IX_Anh san pham_ProductNameId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryImages_ProductInventoryId",
                table: "Anh san pham",
                newName: "IX_Anh san pham_ProductInventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryImages_ProductCategoryId",
                table: "Anh san pham",
                newName: "IX_Anh san pham_ProductCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_San pham trong kho",
                table: "San pham trong kho",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Phan lop san pham",
                table: "Phan lop san pham",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Anh san pham",
                table: "Anh san pham",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Anh san pham_Phan lop san pham_ProductCategoryId",
                table: "Anh san pham",
                column: "ProductCategoryId",
                principalTable: "Phan lop san pham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Anh san pham_ProductNames_ProductNameId",
                table: "Anh san pham",
                column: "ProductNameId",
                principalTable: "ProductNames",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Anh san pham_San pham trong kho_ProductInventoryId",
                table: "Anh san pham",
                column: "ProductInventoryId",
                principalTable: "San pham trong kho",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProducts_Phan lop san pham_ProductCategoryId",
                table: "CategoryProducts",
                column: "ProductCategoryId",
                principalTable: "Phan lop san pham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProducts_San pham trong kho_ProductInventoryId",
                table: "CategoryProducts",
                column: "ProductInventoryId",
                principalTable: "San pham trong kho",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Phan lop san pham_Phan lop san pham_ParentCategoryId",
                table: "Phan lop san pham",
                column: "ParentCategoryId",
                principalTable: "Phan lop san pham",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_San pham trong kho_ProductNames_ProductNameId",
                table: "San pham trong kho",
                column: "ProductNameId",
                principalTable: "ProductNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
