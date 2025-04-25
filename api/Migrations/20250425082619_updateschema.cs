using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class updateschema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_EpisodeId",
                table: "Videos");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_EpisodeId",
                table: "Videos",
                column: "EpisodeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_EpisodeId",
                table: "Videos");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_EpisodeId",
                table: "Videos",
                column: "EpisodeId");
        }
    }
}
