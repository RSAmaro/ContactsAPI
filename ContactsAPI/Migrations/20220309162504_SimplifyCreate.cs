using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsAPI.Migrations
{
    public partial class SimplifyCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_ContactTypes_TypeNameId",
                table: "Contacts");

            migrationBuilder.DropTable(
                name: "ContactTypes");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_TypeNameId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "TypeNameId",
                table: "Contacts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Contacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeNameId",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContactTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_TypeNameId",
                table: "Contacts",
                column: "TypeNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_ContactTypes_TypeNameId",
                table: "Contacts",
                column: "TypeNameId",
                principalTable: "ContactTypes",
                principalColumn: "Id");
        }
    }
}
