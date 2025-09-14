using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStages.Migrations
{
    /// <inheritdoc />
    public partial class AddEtudiantToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EtudiantId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_EtudiantId",
                table: "Documents",
                column: "EtudiantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Etudiants_EtudiantId",
                table: "Documents",
                column: "EtudiantId",
                principalTable: "Etudiants",
                principalColumn: "IdEtudiant",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Etudiants_EtudiantId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_EtudiantId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "EtudiantId",
                table: "Documents");
        }
    }
}
