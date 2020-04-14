using Microsoft.EntityFrameworkCore.Migrations;

namespace esquirebackend.Migrations
{
    public partial class ProjectStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Projects",
                nullable: false,
                defaultValue: (byte)1);

            migrationBuilder.AlterColumn<string>(
                name: "Annotations",
                table: "CodedAnswers",
                nullable: true,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "Annotations",
                table: "CodedAnswers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "[]");
        }
    }
}
