using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobs4Devs.MinimalAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "varchar(240)", nullable: false),
                    Description = table.Column<string>(type: "varchar(1024)", nullable: false),
                    Company = table.Column<string>(type: "varchar(120)", nullable: false),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MinSalary = table.Column<double>(type: "float", nullable: false),
                    MaxSalary = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2022, 3, 19, 10, 8, 44, 719, DateTimeKind.Local).AddTicks(5200))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vacancies");
        }
    }
}
