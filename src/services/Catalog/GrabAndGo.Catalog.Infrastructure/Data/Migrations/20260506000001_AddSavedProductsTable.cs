using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrabAndGo.Catalog.Infrastructure.Data.Migrations
{
    public partial class AddSavedProductsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedProducts_ProductId",
                table: "SavedProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedProducts_UserId_ProductId",
                table: "SavedProducts",
                columns: new[] { "UserId", "ProductId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedProducts");
        }
    }
}
