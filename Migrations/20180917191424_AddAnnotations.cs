using Microsoft.EntityFrameworkCore.Migrations;

namespace esquirebackend.Migrations
{
    public partial class AddHighlight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Annotations",
                table: "CodedAnswers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Annotations",
                table: "CodedAnswers");
        }
    }
}
