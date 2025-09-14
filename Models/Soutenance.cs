using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionStages.Models
{
    public class Soutenance
    {
        [Key]
        public int IdSoutenance { get; set; }
        public DateTime? DateSoutenance { get; set; }
        public string? Lieu { get; set; }
        public string? Jury1 { get; set; }
        public string? Jury2 { get; set; }
        public string? NomSujet { get; set; }          // <-- new
        public string? DescriptionSujet { get; set; }  // <-- new
        public string? StatutSujet { get; set; } = "En attente"; // <-- new

        public double? NoteFinale { get; set; }

        public int StageId { get; set; }
        public Stage Stage { get; set; }
        // ✅ Computed property pour savoir si validé
        [NotMapped]
        public bool EstValide => NoteFinale.HasValue && NoteFinale.Value >= 10;
    }


}
