using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class removeTagCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TagCategories_TagId",
                table: "TagCategories");

            migrationBuilder.DropIndex(
                name: "IX_SeriesCategories_CategoryId",
                table: "SeriesCategories");

            migrationBuilder.CreateIndex(
                name: "IX_TagCategories_TagId",
                table: "TagCategories",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesCategories_CategoryId",
                table: "SeriesCategories",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TagCategories_TagId",
                table: "TagCategories");

            migrationBuilder.DropIndex(
                name: "IX_SeriesCategories_CategoryId",
                table: "SeriesCategories");

            migrationBuilder.CreateIndex(
                name: "IX_TagCategories_TagId",
                table: "TagCategories",
                column: "TagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeriesCategories_CategoryId",
                table: "SeriesCategories",
                column: "CategoryId",
                unique: true);
        }
    }
}
