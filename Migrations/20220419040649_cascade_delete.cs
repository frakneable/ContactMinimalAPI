using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactMinimalAPI.Migrations
{
    public partial class cascade_delete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_People_PersonId",
                table: "Contacts");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_People_PersonId",
                table: "Contacts",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_People_PersonId",
                table: "Contacts");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_People_PersonId",
                table: "Contacts",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id");
        }
    }
}
