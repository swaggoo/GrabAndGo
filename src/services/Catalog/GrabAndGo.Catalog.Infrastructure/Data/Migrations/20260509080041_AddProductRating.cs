using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrabAndGo.Catalog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "TotalRatings",
                table: "Businesses");

            migrationBuilder.AddColumn<float>(
                name: "Rating_CollectionRating",
                table: "Products",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Rating_OverallRating",
                table: "Products",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Rating_QualityRating",
                table: "Products",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Rating_QuantityRating",
                table: "Products",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Rating_TotalRatings",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Rating_VarietyRating",
                table: "Products",
                type: "real",
                nullable: false,
                defaultValue: 0f);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedProducts");

            migrationBuilder.DropColumn(
                name: "Rating_CollectionRating",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Rating_OverallRating",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Rating_QualityRating",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Rating_QuantityRating",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Rating_TotalRatings",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Rating_VarietyRating",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Businesses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "Businesses",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalRatings",
                table: "Businesses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
