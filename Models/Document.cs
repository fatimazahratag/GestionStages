using System.ComponentModel.DataAnnotations;
namespace GestionStages.Models
{
    public class Document
    {
        [Key]
        public int IdDocument { get; set; }
        public string? NomFichier { get; set; }  // <-- nullable
        public string? TypeDocument { get; set; } // <-- nullable
        public DateTime DateDepot { get; set; }
        public string Statut { get; set; } = "En attente"; // valeur par défaut

        public int? StageId { get; set; }
        public Stage Stage { get; set; }

        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }
        public DateTime? DateLimiteRapportFinal { get; set; }


    }

}
