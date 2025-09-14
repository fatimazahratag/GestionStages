using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class RegisterAdminViewModel
    {
        [Required]
        public string NomComplet { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string MotDePasse { get; set; }

        [Required]
        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        [DataType(DataType.Password)]
        public string ConfirmerMotDePasse { get; set; }


        public string Email { get; set; }
        public string Telephone { get; set; }
    }
}
