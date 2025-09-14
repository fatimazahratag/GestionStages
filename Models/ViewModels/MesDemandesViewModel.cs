using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionStages.Models.ViewModels
{
    public class DemandeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un type de document.")]
        public string TypeDocument { get; set; }

        public DateTime? DateDepot { get; set; }

        public string NomFichier { get; set; }

        public string Statut { get; set; }
    }

    public class MesDemandesViewModel
    {
        public DemandeViewModel NouvelleDemande { get; set; } = new DemandeViewModel();
        public List<DemandeViewModel> ListeDemandes { get; set; } = new List<DemandeViewModel>();
    }
}
