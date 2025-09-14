using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class SujetViewModel
    {
        public string NomEncadrant { get; set; } // juste pour affichage

        [Required(ErrorMessage = "Le nom du sujet est requis")]
        [StringLength(200)]
        public string NomSujet { get; set; }

        [Required(ErrorMessage = "La description est requise")]
        public string Description { get; set; }

        public decimal? Note { get; set; } // visible uniquement après soutenance
    }
}
