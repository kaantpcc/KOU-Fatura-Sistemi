using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillDepartment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillValue = table.Column<int>(type: "int", nullable: false),
                    BillImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bills");
        }
    }
}
