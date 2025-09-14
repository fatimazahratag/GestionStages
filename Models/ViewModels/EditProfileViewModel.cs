using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "Nom Complet")]
        public string NomComplet { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Telephone { get; set; }

        public string Filiere { get; set; }
        public string Niveau { get; set; }
        public string Module { get; set; }
    }
}
