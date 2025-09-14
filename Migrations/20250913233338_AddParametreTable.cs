using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStages.Migrations
{
    /// <inheritdoc />
    public partial class AddParametreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateLimiteRapportFinal",
                table: "Documents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateLimiteRapportFinal",
                table: "Documents");
        }
    }
}
