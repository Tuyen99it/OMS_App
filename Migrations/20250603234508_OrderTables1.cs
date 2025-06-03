using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS_App.Migrations
{
    /// <inheritdoc />
    public partial class OrderTables1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedProducts_Orders_OrderedProductId",
                table: "OrderedProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderAddresses_AddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderedProducts_OrderedProductId",
                table: "OrderedProducts");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderedProductId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderedProductId",
                table: "OrderedProducts");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "OrderAddresses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderAddresses_OrderId",
                table: "OrderAddresses",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAddresses_Orders_OrderId",
                table: "OrderAddresses",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderAddresses_Orders_OrderId",
                table: "OrderAddresses");

            migrationBuilder.DropIndex(
                name: "IX_OrderAddresses_OrderId",
                table: "OrderAddresses");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OrderAddresses");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderedProductId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderedProductId",
                table: "OrderedProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedProducts_OrderedProductId",
                table: "OrderedProducts",
                column: "OrderedProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedProducts_Orders_OrderedProductId",
                table: "OrderedProducts",
                column: "OrderedProductId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderAddresses_AddressId",
                table: "Orders",
                column: "AddressId",
                principalTable: "OrderAddresses",
                principalColumn: "Id");
        }
    }
}
