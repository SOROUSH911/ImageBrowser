using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageBrowser.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixOneToManyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_OwnerId",
                table: "Files");

            migrationBuilder.CreateIndex(
                name: "IX_Files_OwnerId",
                table: "Files",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_OwnerId",
                table: "Files");

            migrationBuilder.CreateIndex(
                name: "IX_Files_OwnerId",
                table: "Files",
                column: "OwnerId",
                unique: true);
        }
    }
}
