using Microsoft.EntityFrameworkCore.Migrations;

namespace esquirebackend.Migrations
{
    public partial class reverse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Name",
                table: "Jurisdictions",
                newName: "IX_JurisName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_JurisName",
                table: "Jurisdictions",
                newName: "IX_Name");
        }
    }
}
