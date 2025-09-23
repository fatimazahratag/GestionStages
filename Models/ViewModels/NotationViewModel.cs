namespace GestionStages.Models.ViewModels
{
    public class NotationViewModel
    {
        public int SoutenanceId { get; set; }
        public int SujetId { get; set; }
        public string SujetTitre { get; set; }
        public string EtudiantNom { get; set; }
        public string Filiere { get; set; }  // nouvelle propriété
        public double? Note { get; set; }
    }
}
