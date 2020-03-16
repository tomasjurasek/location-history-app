using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIdentifier",
                table: "UsersLocations");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UsersLocations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersLocations_UserId",
                table: "UsersLocations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersLocations_Users_UserId",
                table: "UsersLocations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersLocations_Users_UserId",
                table: "UsersLocations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UsersLocations_UserId",
                table: "UsersLocations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsersLocations");

            migrationBuilder.AddColumn<string>(
                name: "UserIdentifier",
                table: "UsersLocations",
                type: "text",
                nullable: true);
        }
    }
}
