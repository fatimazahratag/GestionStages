using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStages.Migrations
{
    /// <inheritdoc />
    public partial class AjoutDescriptionSujet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionSujet",
                table: "Stages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "NoteFinale",
                table: "Soutenances",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionSujet",
                table: "Soutenances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomSujet",
                table: "Soutenances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatutSujet",
                table: "Soutenances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionSujet",
                table: "Stages");

            migrationBuilder.DropColumn(
                name: "DescriptionSujet",
                table: "Soutenances");

            migrationBuilder.DropColumn(
                name: "NomSujet",
                table: "Soutenances");

            migrationBuilder.DropColumn(
                name: "StatutSujet",
                table: "Soutenances");

            migrationBuilder.AlterColumn<double>(
                name: "NoteFinale",
                table: "Soutenances",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
