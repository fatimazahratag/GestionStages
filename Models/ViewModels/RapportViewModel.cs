using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GestionStages.Models.ViewModels
{
    public class RapportViewModel
    {
        public string Niveau { get; set; }

        public IFormFile Rapport { get; set; }
        public DateTime? DateLimiteRapportFinal { get; set; }

        public List<Document> ListeRapports { get; set; } = new List<Document>();
        public int Id { get; set; }             
        public string Titre { get; set; }      
        public int EtudiantId { get; set; }
        public string EtudiantNom { get; set; }
        public string Statut { get; set; }       
        public DateTime? DateDepot { get; set; }
        public string NomFichier { get; set; }   
    }
}
