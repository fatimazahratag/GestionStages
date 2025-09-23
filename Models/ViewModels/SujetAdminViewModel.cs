using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class SujetAdminViewModel
    {
        public int IdStage { get; set; }

        public string NomEtudiant { get; set; } 

        public string NomEncadrant { get; set; } = "Non affecté";

        [Required(ErrorMessage = "Le nom du sujet est requis")]
        [StringLength(200)]
        public string NomSujet { get; set; }

        [Required(ErrorMessage = "La description est requise")]
        public string Description { get; set; }

        public string StatutSujet { get; set; } = "En attente";

        public decimal? Note { get; set; }
    }
}
