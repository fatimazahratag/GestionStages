using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace GestionStages.Models
{
    public class Stage
    {
        [Key]
        public int IdStage { get; set; }
        public string TypeStage { get; set; }
        public string? Theme { get; set; }   
        public string? Description { get; set; }

        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public bool ConfirmationEtudiant { get; set; }
        public string NomSignature { get; set; }
        public DateTime DateSoumission { get; set; }
        public bool AutorisationTraitement { get; set; }

        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }

        public int EnseignantId { get; set; }
        public Enseignant Enseignant { get; set; }

        public int OrganismeId { get; set; }
        public Organisme Organisme { get; set; }

        public ICollection<Document> Documents { get; set; }
        public ICollection<Suivi> Suivis { get; set; }
        public ICollection<Validation> Validations { get; set; }
        public Soutenance Soutenance { get; set; }
    }

}
