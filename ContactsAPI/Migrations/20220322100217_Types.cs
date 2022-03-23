using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsAPI.Migrations
{
    public partial class Types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "ContactTypes",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateDeleted",
                table: "ContactTypes",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateUpdated",
                table: "ContactTypes",
                type: "datetimeoffset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ContactTypes");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "ContactTypes");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "ContactTypes");
        }
    }
}
