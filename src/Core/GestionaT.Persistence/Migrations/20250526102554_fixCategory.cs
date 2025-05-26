using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionaT.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ImageId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_CategoryImages_CategoryId",
                table: "CategoryImages");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ImageId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryImages_CategoryId",
                table: "CategoryImages",
                column: "CategoryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CategoryImages_CategoryId",
                table: "CategoryImages");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "Categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryImages_CategoryId",
                table: "CategoryImages",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ImageId",
                table: "Categories",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ImageId",
                table: "Categories",
                column: "ImageId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
