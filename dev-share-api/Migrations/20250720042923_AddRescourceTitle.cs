using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dev_share_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRescourceTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Resources",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Resources");
        }
    }
}
