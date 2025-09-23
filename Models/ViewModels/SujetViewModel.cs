using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class SujetViewModel
    {
        public int IdStage { get; set; }
        public int IdSoutenance { get; set; }

        public string NomEncadrant { get; set; } // juste pour affichage
        public string NomEtudiant { get; set; } // juste pour affichage

        public string EtudiantNom { get; set; }

        [Required(ErrorMessage = "Le nom du sujet est requis")]
        [StringLength(200)]
        public string NomSujet { get; set; }

        [Required(ErrorMessage = "La description est requise")]
        public string DescriptionSujet { get; set; }
        public string Description { get; set; }

        public string StatutSujet { get; set; }
        public string Statut { get; set; } = "En attente";


        public decimal? Note { get; set; } // visible uniquement après soutenance

        public int Id { get; set; }
        public string Titre { get; set; }

    }
}
