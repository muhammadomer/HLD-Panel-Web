using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityProject.Data.Migrations
{
    public partial class Extended : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAlias",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAlias",
                table: "AspNetUsers");
        }
    }
}
