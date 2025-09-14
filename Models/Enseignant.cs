using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models
{
    public class Enseignant
    {
        [Key]
        public int IdEnseignant { get; set; }
        public string NomEnseignant { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string DepartementAttache { get; set; }
        public string Filiere { get; set; }

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }

        public ICollection<Stage> Stages { get; set; }
        public ICollection<Suivi> Suivis { get; set; }
    }

}
