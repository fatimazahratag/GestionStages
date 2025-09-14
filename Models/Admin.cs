using System.ComponentModel.DataAnnotations;
namespace GestionStages.Models
{
    public class Admin
    {
        [Key]
        public int IdAdmin { get; set; }
        public string NomComplet { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Password { get; set; }

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }
    }

}
