namespace GestionStages.Models.ViewModels
{
    public class EtudiantViewModel
    {
        public int IdEtudiant { get; set; }
        public string NomComplet { get; set; }
        public string CNE { get; set; }
        public int? StageId { get; set; }       
        public string Sujet { get; set; }       
        public string StatutSujet { get; set; } 
    }
}
