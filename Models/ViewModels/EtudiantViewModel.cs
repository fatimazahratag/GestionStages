namespace GestionStages.Models.ViewModels
{
    public class EtudiantViewModel
    {
        public int IdEtudiant { get; set; }
        public string NomComplet { get; set; }
        public string CNE { get; set; }
        public int? StageId { get; set; }        // id du stage si existant
        public string Sujet { get; set; }       // nom du sujet (Soutenance.NomSujet)
        public string StatutSujet { get; set; } // "Validé"/"Refusé"/"En attente"
    }
}
