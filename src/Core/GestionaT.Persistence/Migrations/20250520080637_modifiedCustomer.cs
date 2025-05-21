using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionaT.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class modifiedCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AspNetUsers_UserId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Businesses_BusinessId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Members");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserId_BusinessId",
                table: "Members",
                newName: "IX_Members_UserId_BusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_RoleId",
                table: "Members",
                newName: "IX_Members_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_BusinessId",
                table: "Members",
                newName: "IX_Members_BusinessId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rfc",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Businesses_BusinessId",
                table: "Members",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Roles_RoleId",
                table: "Members",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Businesses_BusinessId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Roles_RoleId",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Rfc",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "Members",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_Members_UserId_BusinessId",
                table: "Users",
                newName: "IX_Users_UserId_BusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_Members_RoleId",
                table: "Users",
                newName: "IX_Users_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Members_BusinessId",
                table: "Users",
                newName: "IX_Users_BusinessId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AspNetUsers_UserId",
                table: "Users",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Businesses_BusinessId",
                table: "Users",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
