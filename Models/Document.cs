using System.ComponentModel.DataAnnotations;
namespace GestionStages.Models
{
    public class Document
    {
        [Key]
        public int IdDocument { get; set; }
        public string? NomFichier { get; set; }  
        public string? TypeDocument { get; set; } 
        public DateTime DateDepot { get; set; }
        public string Statut { get; set; } = "En attente";

        public int? StageId { get; set; }
        public Stage Stage { get; set; }

        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }
        public DateTime? DateLimiteRapportFinal { get; set; }


    }

}
