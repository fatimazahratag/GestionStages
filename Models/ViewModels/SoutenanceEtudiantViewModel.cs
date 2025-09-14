namespace GestionStages.Models.ViewModels
{
    public class SoutenanceEtudiantViewModel
    {
        public string NomSujet { get; set; }
        public string Description { get; set; }

        public DateTime? DateSoutenance { get; set; }
        public string? Lieu { get; set; }

        public string Jury1 { get; set; }
        public string Jury2 { get; set; }

        public double? NoteFinale { get; set; }

        public string Statut => (NoteFinale.HasValue && NoteFinale.Value >= 10) ? "Validé ✅"
                            : (NoteFinale.HasValue ? "Non validé ❌" : "En attente ⏳");
    }
}
