using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStages.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organismes",
                columns: table => new
                {
                    IdOrganisme = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomOrganisme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecteurActivite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomResponsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FonctionResponsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailResponsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelephoneResponsable = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organismes", x => x.IdOrganisme);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomUtilisateur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotDePasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.IdUtilisateur);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    IdAdmin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomComplet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.IdAdmin);
                    table.ForeignKey(
                        name: "FK_Admins_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "IdUtilisateur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enseignants",
                columns: table => new
                {
                    IdEnseignant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomEnseignant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartementAttache = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filiere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enseignants", x => x.IdEnseignant);
                    table.ForeignKey(
                        name: "FK_Enseignants_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "IdUtilisateur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Etudiants",
                columns: table => new
                {
                    IdEtudiant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomComplet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CNE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filiere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Niveau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etudiants", x => x.IdEtudiant);
                    table.ForeignKey(
                        name: "FK_Etudiants_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "IdUtilisateur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stages",
                columns: table => new
                {
                    IdStage = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeStage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Theme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmationEtudiant = table.Column<bool>(type: "bit", nullable: false),
                    NomSignature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSoumission = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AutorisationTraitement = table.Column<bool>(type: "bit", nullable: false),
                    EtudiantId = table.Column<int>(type: "int", nullable: false),
                    EnseignantId = table.Column<int>(type: "int", nullable: false),
                    OrganismeId = table.Column<int>(type: "int", nullable: false),
                    EnseignantIdEnseignant = table.Column<int>(type: "int", nullable: true),
                    EtudiantIdEtudiant = table.Column<int>(type: "int", nullable: true),
                    OrganismeIdOrganisme = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.IdStage);
                    table.ForeignKey(
                        name: "FK_Stages_Enseignants_EnseignantId",
                        column: x => x.EnseignantId,
                        principalTable: "Enseignants",
                        principalColumn: "IdEnseignant",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stages_Enseignants_EnseignantIdEnseignant",
                        column: x => x.EnseignantIdEnseignant,
                        principalTable: "Enseignants",
                        principalColumn: "IdEnseignant");
                    table.ForeignKey(
                        name: "FK_Stages_Etudiants_EtudiantId",
                        column: x => x.EtudiantId,
                        principalTable: "Etudiants",
                        principalColumn: "IdEtudiant",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stages_Etudiants_EtudiantIdEtudiant",
                        column: x => x.EtudiantIdEtudiant,
                        principalTable: "Etudiants",
                        principalColumn: "IdEtudiant");
                    table.ForeignKey(
                        name: "FK_Stages_Organismes_OrganismeId",
                        column: x => x.OrganismeId,
                        principalTable: "Organismes",
                        principalColumn: "IdOrganisme",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stages_Organismes_OrganismeIdOrganisme",
                        column: x => x.OrganismeIdOrganisme,
                        principalTable: "Organismes",
                        principalColumn: "IdOrganisme");
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    IdDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateDepot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.IdDocument);
                    table.ForeignKey(
                        name: "FK_Documents_Stages_StageId",
                        column: x => x.StageId,
                        principalTable: "Stages",
                        principalColumn: "IdStage",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Soutenances",
                columns: table => new
                {
                    IdSoutenance = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateSoutenance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lieu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jury1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jury2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoteFinale = table.Column<double>(type: "float", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soutenances", x => x.IdSoutenance);
                    table.ForeignKey(
                        name: "FK_Soutenances_Stages_StageId",
                        column: x => x.StageId,
                        principalTable: "Stages",
                        principalColumn: "IdStage",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suivis",
                columns: table => new
                {
                    IdSuivi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentairePedagogique = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoteStage = table.Column<double>(type: "float", nullable: false),
                    DateSuivi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    EnseignantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suivis", x => x.IdSuivi);
                    table.ForeignKey(
                        name: "FK_Suivis_Enseignants_EnseignantId",
                        column: x => x.EnseignantId,
                        principalTable: "Enseignants",
                        principalColumn: "IdEnseignant",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Suivis_Stages_StageId",
                        column: x => x.StageId,
                        principalTable: "Stages",
                        principalColumn: "IdStage",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Validations",
                columns: table => new
                {
                    IdValidation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Etape = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarque = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateValidation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    ValidePar = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Validations", x => x.IdValidation);
                    table.ForeignKey(
                        name: "FK_Validations_Stages_StageId",
                        column: x => x.StageId,
                        principalTable: "Stages",
                        principalColumn: "IdStage",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UtilisateurId",
                table: "Admins",
                column: "UtilisateurId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_StageId",
                table: "Documents",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_Enseignants_UtilisateurId",
                table: "Enseignants",
                column: "UtilisateurId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Etudiants_UtilisateurId",
                table: "Etudiants",
                column: "UtilisateurId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Soutenances_StageId",
                table: "Soutenances",
                column: "StageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stages_EnseignantId",
                table: "Stages",
                column: "EnseignantId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_EnseignantIdEnseignant",
                table: "Stages",
                column: "EnseignantIdEnseignant");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_EtudiantId",
                table: "Stages",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_EtudiantIdEtudiant",
                table: "Stages",
                column: "EtudiantIdEtudiant");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_OrganismeId",
                table: "Stages",
                column: "OrganismeId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_OrganismeIdOrganisme",
                table: "Stages",
                column: "OrganismeIdOrganisme");

            migrationBuilder.CreateIndex(
                name: "IX_Suivis_EnseignantId",
                table: "Suivis",
                column: "EnseignantId");

            migrationBuilder.CreateIndex(
                name: "IX_Suivis_StageId",
                table: "Suivis",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_Validations_StageId",
                table: "Validations",
                column: "StageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Soutenances");

            migrationBuilder.DropTable(
                name: "Suivis");

            migrationBuilder.DropTable(
                name: "Validations");

            migrationBuilder.DropTable(
                name: "Stages");

            migrationBuilder.DropTable(
                name: "Enseignants");

            migrationBuilder.DropTable(
                name: "Etudiants");

            migrationBuilder.DropTable(
                name: "Organismes");

            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
