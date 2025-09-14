using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GestionStages.Models.ViewModels
{
    public class RapportViewModel
    {
        public string Niveau { get; set; }

        // ✅ fichier uploadé
        public IFormFile Rapport { get; set; }
        public DateTime? DateLimiteRapportFinal { get; set; }

        // ✅ liste rapports pour affichage
        public List<Document> ListeRapports { get; set; } = new List<Document>();
    }
}
