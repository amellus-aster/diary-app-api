using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiaryApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDiaryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "content",
                table: "Diaries",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Diaries",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Diaries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Diaries");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Diaries",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Diaries",
                newName: "Updated");
        }
    }
}
