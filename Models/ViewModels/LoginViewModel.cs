using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string NomUtilisateur { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string MotDePasse { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
