using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class RegisterEnseignantViewModel
    {
        [Required]
        public string NomEnseignant { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string MotDePasse { get; set; }

        [Required]
        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        [DataType(DataType.Password)]
        public string ConfirmerMotDePasse { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string DepartementAttache { get; set; }
        public string Filiere { get; set; }
    }
}
