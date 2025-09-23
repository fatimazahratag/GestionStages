namespace GestionStages.Models.ViewModels
{
    public class EncadrantViewModel
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Departement { get; set; }

        public int SujetsCount { get; set; }
    }
}
