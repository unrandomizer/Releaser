using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Releaser.Migrations
{
    public partial class NewMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ReleasedAt = table.Column<DateTime>(nullable: true),
                    CanceledAt = table.Column<DateTime>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    RefCode = table.Column<string>(nullable: true),
                    AmountRub = table.Column<decimal>(nullable: false),
                    AmountBtc = table.Column<decimal>(nullable: false),
                    IsBuying = table.Column<bool>(nullable: false),
                    MarkedAsPaid = table.Column<bool>(nullable: false),
                    IsMessageSanded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}
