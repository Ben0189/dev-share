using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dev_share_api.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceFieldv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Resources_ResourceId",
                table: "Resources",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInsights_Resources_ResourceId",
                table: "UserInsights",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInsights_Resources_ResourceId",
                table: "UserInsights");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Resources_ResourceId",
                table: "Resources");
        }
    }
}
