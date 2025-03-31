using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS_App.Migrations
{
    /// <inheritdoc />
    public partial class AdđPost1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Post",
                newName: "Content1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content1",
                table: "Post",
                newName: "Content");
        }
    }
}
