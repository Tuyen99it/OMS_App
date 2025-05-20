using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS_App.Migrations
{
    /// <inheritdoc />
    public partial class updateUserImage1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserImages_AspNetUsers_AppUserId",
                table: "UserImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserImages",
                table: "UserImages");

            migrationBuilder.RenameTable(
                name: "UserImages",
                newName: "User Image");

            migrationBuilder.RenameIndex(
                name: "IX_UserImages_AppUserId",
                table: "User Image",
                newName: "IX_User Image_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User Image",
                table: "User Image",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User Image_AspNetUsers_AppUserId",
                table: "User Image",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User Image_AspNetUsers_AppUserId",
                table: "User Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User Image",
                table: "User Image");

            migrationBuilder.RenameTable(
                name: "User Image",
                newName: "UserImages");

            migrationBuilder.RenameIndex(
                name: "IX_User Image_AppUserId",
                table: "UserImages",
                newName: "IX_UserImages_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserImages",
                table: "UserImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserImages_AspNetUsers_AppUserId",
                table: "UserImages",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
