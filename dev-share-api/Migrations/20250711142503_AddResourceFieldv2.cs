using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dev_share_api.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceFieldv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserInsights_ResourceId",
                table: "UserInsights",
                column: "ResourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInsights_ResourceId",
                table: "UserInsights");
        }
    }
}
