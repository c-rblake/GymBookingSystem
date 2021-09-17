using Microsoft.EntityFrameworkCore.Migrations;

namespace GymBookingSystem.Data.Migrations
{
    public partial class JoinTableIDRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ApplicationUserGymClass");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ApplicationUserGymClass",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
