using Microsoft.EntityFrameworkCore.Migrations;

namespace esquirebackend.Migrations
{
    public partial class JurisdNameIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Jurisdictions",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Name",
                table: "Jurisdictions",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Name",
                table: "Jurisdictions");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Jurisdictions",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
