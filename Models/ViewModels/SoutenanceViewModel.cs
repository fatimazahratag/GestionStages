// Models/ViewModels/SoutenanceViewModel.cs
using System.Collections.Generic;

namespace GestionStages.Models.ViewModels
{
    public class SoutenanceViewModel
    {
        public IEnumerable<Soutenance> Soutenances { get; set; }
        public IEnumerable<Enseignant> Enseignants { get; set; }
        public List<Etudiant> Etudiants { get; set; } = new List<Etudiant>();
        public List<string> Niveaux { get; set; }

        public List<Soutenance> Soutenances4eme => Soutenances.Where(s => s.Stage.Etudiant.Niveau == "4ème année").ToList();
        public List<Soutenance> Soutenances5eme => Soutenances.Where(s => s.Stage.Etudiant.Niveau == "5ème année").ToList();
        public int Id { get; set; }
        public string SujetTitre { get; set; }
        public string EtudiantNom { get; set; }
        public DateTime Date { get; set; }
        public string Salle { get; set; }
        public string Jury { get; set; }
    }
}
