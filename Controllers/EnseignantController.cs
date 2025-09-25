using GestionStages.Data;
using GestionStages.Models;
using GestionStages.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using SelectPdf;
using System.IO.Compression;

namespace GestionStages.Controllers
{
    public class EnseignantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EnseignantController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }



        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants
                .Include(e => e.Stages)
                .ThenInclude(s => s.Etudiant)
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (enseignant == null) return NotFound();

            var stages = enseignant.Stages ?? new List<Stage>();

            var etudiantsIds = stages
                .Where(s => s.EtudiantId != null)
                .Select(s => s.EtudiantId)
                .Distinct()
                .ToList();

            ViewBag.TotalEtudiants = etudiantsIds.Count;
            ViewBag.TotalStages = stages.Count;
            ViewBag.TotalSoutenances = _context.Soutenances
                .Count(s => s.Stage.EnseignantId == enseignant.IdEnseignant);

            ViewBag.Filieres = new List<string> { "1ère année", "2ème année", "3ème année" };
            ViewBag.StageCountsByFiliere = new Dictionary<string, int> { { "1ère année", 0 }, { "2ème année", 0 }, { "3ème année", 0 } };

            return View(enseignant);
        }




        public IActionResult Profil()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants
                .Include(e => e.Utilisateur)
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (enseignant == null) return NotFound();

            return View(enseignant);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            return View("Edit", enseignant); 
        }

        [HttpPost]
        public IActionResult Edit(int IdEnseignant, string NomEnseignant, string Email, string Telephone, string DepartementAttache, string Filiere)
        {
            var enseignant = _context.Enseignants
                .Include(e => e.Utilisateur)
                .FirstOrDefault(e => e.IdEnseignant == IdEnseignant);

            if (enseignant == null) return NotFound();

            enseignant.NomEnseignant = NomEnseignant;
            enseignant.Email = Email;
            enseignant.Telephone = Telephone;
            enseignant.DepartementAttache = DepartementAttache;
            enseignant.Filiere = Filiere;

            if (enseignant.Utilisateur != null)
                enseignant.Utilisateur.NomUtilisateur = Email;

            _context.SaveChanges();
            TempData["Success"] = "Profil mis à jour avec succès.";

            return RedirectToAction("Profil");
        }

        [HttpPost]
        public IActionResult ChangePassword(int UtilisateurId, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["Error"] = "Les mots de passe ne correspondent pas.";
                return RedirectToAction("Profil");
            }

            var user = _context.Utilisateurs.FirstOrDefault(u => u.IdUtilisateur == UtilisateurId);
            if (user == null)
            {
                TempData["Error"] = "Utilisateur introuvable.";
                return RedirectToAction("Profil");
            }

            user.MotDePasse = NewPassword;

            _context.SaveChanges();

            TempData["Success"] = "Mot de passe changé avec succès.";
            return RedirectToAction("Profil");
        }

        public IActionResult EditProfil()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            return View(enseignant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfil(Enseignant model)
        {
            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == model.UtilisateurId);
            var user = _context.Utilisateurs.FirstOrDefault(u => u.IdUtilisateur == model.UtilisateurId);

            if (enseignant == null || user == null)
            {
                TempData["Error"] = "Utilisateur introuvable.";
                return RedirectToAction("EditProfil");
            }

            enseignant.NomEnseignant = model.NomEnseignant;
            enseignant.Email = model.Email;
            enseignant.Telephone = model.Telephone;
            enseignant.DepartementAttache = model.DepartementAttache;
            enseignant.Filiere = model.Filiere;


            _context.SaveChanges();

            TempData["Success"] = "Profil mis à jour avec succès.";
            return RedirectToAction("Profil");
        }

        public IActionResult Sujets()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants
                .Include(e => e.Stages)
                .ThenInclude(s => s.Soutenance)
                .Include(e => e.Stages)
                .ThenInclude(s => s.Etudiant)
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (enseignant == null) return NotFound();

            var sujets = enseignant.Stages
                .Where(s => s.Soutenance != null) 
                .Select(s => new SujetViewModel
                {
                    Id = s.Soutenance.IdSoutenance,
                    Titre = s.Soutenance.NomSujet,
                    Description = s.Soutenance.DescriptionSujet,
                    Statut = s.Soutenance.StatutSujet,
                    EtudiantNom = s.Etudiant.NomComplet
                })
                .ToList();

            return View(sujets);
        }

        public IActionResult Soutenances()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var soutenances = _context.Soutenances
                .Include(s => s.Stage)
                .ThenInclude(st => st.Etudiant)
                .Include(s => s.Stage)
                .ThenInclude(st => st.Enseignant)
                .Where(s => s.Stage.Enseignant.UtilisateurId == userId)
                .ToList();

            var model = soutenances.Select(s => new SoutenanceViewModel
            {
                Id = s.IdSoutenance,
                SujetTitre = s.NomSujet ?? "Pas encore défini",
                EtudiantNom = s.Stage?.Etudiant?.NomComplet ?? "Inconnu",
                Date = s.DateSoutenance ?? DateTime.MinValue,
                Salle = s.Lieu ?? "Non défini",
                Jury = $"{s.Jury1 ?? "Non défini"} / {s.Jury2 ?? "Non défini"}"
            }).ToList();

            return View(model);
        }






        [Route("Enseignant/Etudiants")]
        public IActionResult MesEtudiants()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            var etudiants = _context.Stages
                .Where(s => s.EnseignantId == enseignant.IdEnseignant)
                .Select(s => new MesEtudiantsViewModel
                {
                    EtudiantNom = s.Etudiant != null && s.Etudiant.NomComplet != null ? s.Etudiant.NomComplet : "Inconnu",
                    EtudiantCNE = s.Etudiant != null && s.Etudiant.CNE != null ? s.Etudiant.CNE : "Inconnu",
                    EtudiantEmail = s.Etudiant != null && s.Etudiant.Email != null ? s.Etudiant.Email : "Inconnu",
                    EtudiantTelephone = s.Etudiant != null && s.Etudiant.Telephone != null ? s.Etudiant.Telephone : "Inconnu",
                    EtudiantFiliere = s.Etudiant != null && s.Etudiant.Filiere != null ? s.Etudiant.Filiere : "Inconnu",
                    EtudiantNiveau = s.Etudiant != null && s.Etudiant.Niveau != null ? s.Etudiant.Niveau : "Inconnu",

                    SujetTitre = s.Soutenance != null && s.Soutenance.NomSujet != null ? s.Soutenance.NomSujet : "Pas encore défini",
                    SujetDescription = s.Soutenance != null && s.Soutenance.DescriptionSujet != null ? s.Soutenance.DescriptionSujet : "Pas encore défini",
                    NoteFinale = s.Soutenance != null && s.Soutenance.NoteFinale.HasValue
                                    ? s.Soutenance.NoteFinale.Value.ToString()
                                    : "Pas encore noté"
                })
                .ToList();

            return View("MesEtudiants", etudiants);
        }








        public IActionResult Rapports()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (enseignant == null) return NotFound();

            var etudiantIds = _context.Stages
                .Where(st => st.EnseignantId == enseignant.IdEnseignant)
                .Select(st => st.EtudiantId)
                .Distinct()
                .ToList();

            var documents = _context.Documents
    .Include(d => d.Etudiant)
    .Where(d => etudiantIds.Contains(d.EtudiantId))
    .OrderByDescending(d => d.DateDepot)
    .ToList();


            var vm = documents.Select(d => new RapportViewModel
            {
                Id = d.IdDocument,
                Titre = d.TypeDocument ?? d.NomFichier ?? "Document",
                EtudiantId = d.EtudiantId,
                EtudiantNom = d.Etudiant?.NomComplet ?? "",
                Statut = d.Statut ?? "En attente",
                DateDepot = d.DateDepot,
                NomFichier = d.NomFichier
            }).ToList();

            return View("Rapports", vm); 
        }
        [HttpPost]
        public IActionResult ChangerStatut(int id, string statut)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var document = _context.Documents
                .Include(d => d.Etudiant)
                .FirstOrDefault(d => d.IdDocument == id);

            if (document == null) return NotFound();

            document.Statut = statut;
            _context.SaveChanges();

            TempData["Success"] = $"Le rapport a été {statut.ToLower()} avec succès.";
            return RedirectToAction("Rapports");
        }
        public IActionResult Telecharger(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            var document = _context.Documents
                .Include(d => d.Etudiant)
                .FirstOrDefault(d =>
                    d.IdDocument == id &&
                    d.TypeDocument != null &&
                    d.TypeDocument.Replace(" ", "").Trim().ToLower() == "rapportfinal" &&
                    d.Etudiant != null &&
                    _context.Stages.Any(s => s.EtudiantId == d.EtudiantId
                                             && s.EnseignantId == enseignant.IdEnseignant)
                );

            if (document == null)
                return NotFound("Document introuvable ou vous n'avez pas la permission de le télécharger.");

            var filePath = Path.Combine(_env.WebRootPath, "rapports", document.NomFichier);


            if (!System.IO.File.Exists(filePath))
                return NotFound("Fichier introuvable sur le serveur.");

            return PhysicalFile(filePath, "application/pdf", document.NomFichier);
        }


        public IActionResult TelechargerRapport(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null)
                return NotFound();

            var document = _context.Documents
                .Include(d => d.Etudiant)
                .FirstOrDefault(d => d.IdDocument == id);

            if (document == null)
                return NotFound("Document introuvable.");

            if (document.TypeDocument.Trim().ToLower() != "rapportfinal")
                return BadRequest("Ce document n'est pas un rapport final.");

            bool isEncadre = _context.Stages.Any(s => s.EtudiantId == document.EtudiantId
                                                      && s.EnseignantId == enseignant.IdEnseignant);
            if (!isEncadre)
                return Forbid("Vous n'avez pas la permission de télécharger ce document.");

            var filePath = Path.Combine(_env.WebRootPath, "rapports", document.NomFichier);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Fichier introuvable sur le serveur.");

            return PhysicalFile(filePath, "application/pdf", document.NomFichier);
        }


        [HttpGet]
        public IActionResult Notation(string filiere)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            ViewBag.Filieres = _context.Etudiants
                .Select(e => e.Filiere)
                .Where(f => f != null && f != "") 
                .Distinct()
                .OrderBy(f => f)
                .ToList();

            ViewBag.SelectedFiliere = filiere;

            var query = _context.Stages
                .Include(st => st.Etudiant)
                .Include(st => st.Soutenance)
                .Where(st => st.EnseignantId == enseignant.IdEnseignant);

            if (!string.IsNullOrEmpty(filiere))
                query = query.Where(st => st.Etudiant.Filiere == filiere);

            var model = query
       .Where(st => st.Soutenance != null) 
       .Select(st => new NotationViewModel
       {
           SoutenanceId = st.Soutenance.IdSoutenance,
           EtudiantNom = st.Etudiant.NomComplet,
           Filiere = st.Etudiant.Filiere,
           SujetTitre = st.Soutenance.NomSujet ?? "Sujet non défini",
           Note = st.Soutenance.NoteFinale 
       })
       .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult EnregistrerNote(int SoutenanceId, double Note, string filiere)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var soutenance = _context.Soutenances
                .Include(s => s.Stage)
    .FirstOrDefault(s => s.IdSoutenance == SoutenanceId);

            if (soutenance != null)
            {
                soutenance.NoteFinale = Note;
                _context.SaveChanges();
                TempData["Success"] = "Note ajoutée avec succès.";
            }

            return RedirectToAction("Notation", new { filiere });
        }

    }
}
