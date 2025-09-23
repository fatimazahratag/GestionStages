namespace GestionStages.Models.ViewModels
{
    public class MesEtudiantsViewModel
    {
        public string EtudiantNom { get; set; }
        public string EtudiantCNE { get; set; }
        public string EtudiantEmail { get; set; }
        public string EtudiantTelephone { get; set; }
        public string EtudiantFiliere { get; set; }
        public string EtudiantNiveau { get; set; }

        public string SujetTitre { get; set; }
        public string SujetDescription { get; set; }

        public string RapportTitre { get; set; }
        public string StatutRapport { get; set; }
        public string NoteFinale { get; set; }
    }
}
