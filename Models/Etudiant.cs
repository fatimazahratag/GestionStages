using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models
{
    public class Etudiant
    {
        [Key]
        public int IdEtudiant { get; set; }
        public string NomComplet { get; set; }
        public string CNE { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Filiere { get; set; }
        public string Niveau { get; set; }
        public string Module { get; set; }

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }

        public ICollection<Stage> Stages { get; set; }
        public ICollection<Document> Documents { get; set; }

    }

}
