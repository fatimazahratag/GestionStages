using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class RegisterEtudiantViewModel
    {
        [Required]
        public string NomComplet { get; set; }

        [Required]
        public string CNE { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Telephone { get; set; }

        [Required]
        public string Filiere { get; set; }

        [Required]
        public string Niveau { get; set; }

        [Required]
        public string Module { get; set; }

        [Required]
        public string MotDePasse { get; set; }

        [Required]
        [Compare("MotDePasse")]
        public string ConfirmerMotDePasse { get; set; }
    }
}
