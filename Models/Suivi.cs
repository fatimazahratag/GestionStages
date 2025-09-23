using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models
{
    public class Suivi
    {
        [Key]
        public int IdSuivi { get; set; }
        public string? CommentairePedagogique { get; set; } 
        public double? NoteStage { get; set; } 
        public DateTime DateSuivi { get; set; }

        public int StageId { get; set; }
        public Stage Stage { get; set; }

        public int EnseignantId { get; set; }
        public Enseignant Enseignant { get; set; }
    }

}
