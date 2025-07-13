using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dev_share_api.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Insight",
                table: "UserInsights");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "UserInsights",
                newName: "Content");

            migrationBuilder.AddColumn<long>(
                name: "ResourceId",
                table: "UserInsights",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeUrl",
                table: "Resources",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ResourceId",
                table: "Resources",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_NormalizeUrl",
                table: "Resources",
                column: "NormalizeUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ResourceId",
                table: "Resources",
                column: "ResourceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resources_NormalizeUrl",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Resources_ResourceId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "UserInsights");

            migrationBuilder.DropColumn(
                name: "NormalizeUrl",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "UserInsights",
                newName: "Url");

            migrationBuilder.AddColumn<string>(
                name: "Insight",
                table: "UserInsights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
