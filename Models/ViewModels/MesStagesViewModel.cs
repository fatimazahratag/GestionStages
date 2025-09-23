namespace GestionStages.Models.ViewModels
{
    public class MesStagesViewModel
    {
        public int IdStage { get; set; }
        public string TypeStage { get; set; }
        public string Theme { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public bool ConfirmationEtudiant { get; set; } 

        public string Etat { get; set; } 

        public string NomEnseignant { get; set; }
        public string NomOrganisme { get; set; }
        public string VilleOrganisme { get; set; }
    }
}
