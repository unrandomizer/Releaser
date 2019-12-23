using Microsoft.EntityFrameworkCore.Migrations;

namespace Releaser.Migrations
{
    public partial class ContactUpd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMessageSanded",
                table: "Contacts");

            migrationBuilder.AddColumn<bool>(
                name: "IsMessageSent",
                table: "Contacts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMessageSent",
                table: "Contacts");

            migrationBuilder.AddColumn<bool>(
                name: "IsMessageSanded",
                table: "Contacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
