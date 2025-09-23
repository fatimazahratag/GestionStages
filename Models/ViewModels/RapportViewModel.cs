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
        public int Id { get; set; }              // IdDocument
        public string Titre { get; set; }        // nom ou titre du document
        public int EtudiantId { get; set; }
        public string EtudiantNom { get; set; }
        public string Statut { get; set; }       // Ex: "En attente", "Validé"
        public DateTime? DateDepot { get; set; }
        public string NomFichier { get; set; }   // nom du fichier si stocké
    }
}
