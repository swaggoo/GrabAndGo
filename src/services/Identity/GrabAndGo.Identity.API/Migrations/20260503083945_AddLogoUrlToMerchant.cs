using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrabAndGo.Identity.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLogoUrlToMerchant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "AspNetUsers");
        }
    }
}
