using Microsoft.EntityFrameworkCore.Migrations;

namespace esquirebackend.Migrations
{
    public partial class changeIDXName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_JurisName",
                table: "Jurisdictions",
                newName: "IX_JurisdictionName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_JurisdictionName",
                table: "Jurisdictions",
                newName: "IX_JurisName");
        }
    }
}
