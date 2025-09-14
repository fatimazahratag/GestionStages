using System;
using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class StageDemandeViewModel
    {
        // ORGANISME
        [Required(ErrorMessage = "Veuillez sélectionner un enseignant")]
        public int EnseignantId { get; set; }

        [Required]
        public string Entreprise { get; set; }

        [Required]
        public string DepartementOrganisme { get; set; }

        [Required]
        public string SecteurActiviteOrganisme { get; set; }

        [Required]
        public string AdresseOrganisme { get; set; }

        [Required]
        public string VilleOrganisme { get; set; }

        [Required]
        public string NomResponsableOrganisme { get; set; }

        [Required]
        public string FonctionResponsableOrganisme { get; set; }

        [Required]
        [EmailAddress]
        public string EmailResponsableOrganisme { get; set; }

        [Required]
        [Phone]
        public string TelephoneResponsableOrganisme { get; set; }

        // STAGE
        [Required]
        public string Sujet { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }

        public string TypeStage { get; set; } // Dropdown option

        public bool AutorisationTraitement { get; set; }
    }
}
