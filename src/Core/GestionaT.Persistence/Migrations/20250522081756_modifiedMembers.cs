using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionaT.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class modifiedMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Active",
                table: "Members",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Members");
        }
    }
}
