using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS_App.Migrations
{
    /// <inheritdoc />
    public partial class editInventoryImageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrlPath",
                table: "InventoryImages",
                newName: "RelativeImageUrlPath");

            migrationBuilder.AddColumn<string>(
                name: "AbsoluteImageUrlPath",
                table: "InventoryImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsoluteImageUrlPath",
                table: "InventoryImages");

            migrationBuilder.RenameColumn(
                name: "RelativeImageUrlPath",
                table: "InventoryImages",
                newName: "ImageUrlPath");
        }
    }
}
