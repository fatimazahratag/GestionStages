using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models
{
    public class Organisme
    {
        [Key]
        public int IdOrganisme { get; set; }
        public string NomOrganisme { get; set; }
        public string Departement { get; set; }
        public string SecteurActivite { get; set; }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string NomResponsable { get; set; }
        public string FonctionResponsable { get; set; }
        public string EmailResponsable { get; set; }
        public string TelephoneResponsable { get; set; }

        public ICollection<Stage> Stages { get; set; }
    }

}
