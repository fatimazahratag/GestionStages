using System.ComponentModel.DataAnnotations;
namespace GestionStages.Models
{
    public class Utilisateur
    {
        [Key]
        public int IdUtilisateur { get; set; }
        public string NomUtilisateur { get; set; }
        public string MotDePasse { get; set; }
        public string Role { get; set; }

        public Etudiant Etudiant { get; set; }
        public Enseignant Enseignant { get; set; }
        public Admin Admin { get; set; }
    }

}
