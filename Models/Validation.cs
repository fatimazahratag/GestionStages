using System;
using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models
{
    public class Validation
    {
        [Key]
        public int IdValidation { get; set; }
        public string Etape { get; set; }
        public string Statut { get; set; }
        public string? Remarque { get; set; }   // <-- nullable
        public DateTime DateValidation { get; set; }

        public int StageId { get; set; }
        public Stage Stage { get; set; }

        public string ValidePar { get; set; } 
    }
}
