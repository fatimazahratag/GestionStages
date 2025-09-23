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



        // ----------- Dashboard -----------
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

            // IDs of students safely (non-nullable)
            var etudiantsIds = stages
                .Where(s => s.EtudiantId != null)
                .Select(s => s.EtudiantId)
                .Distinct()
                .ToList();

            // --- Cards ---
            ViewBag.TotalEtudiants = etudiantsIds.Count;
            ViewBag.TotalStages = stages.Count;
            ViewBag.TotalSoutenances = _context.Soutenances
                .Count(s => s.Stage.EnseignantId == enseignant.IdEnseignant);

            // --- Recent Documents ---
            

            // --- Documents status for charts ---
        

            // Optional: Filieres and stages per filiere for charts
            ViewBag.Filieres = new List<string> { "1ère année", "2ème année", "3ème année" };
            ViewBag.StageCountsByFiliere = new Dictionary<string, int> { { "1ère année", 0 }, { "2ème année", 0 }, { "3ème année", 0 } };

            return View(enseignant);
        }




        // ----------- Profil -----------
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

        // ----------- Edit Profil -----------
        [HttpGet]
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            return View("Edit", enseignant); // Edit.cshtml
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

            // Optionnel : hasher le mot de passe si nécessaire
            user.MotDePasse = NewPassword; // ou HasherPassword(NewPassword) si tu utilises un hash

            _context.SaveChanges();

            TempData["Success"] = "Mot de passe changé avec succès.";
            return RedirectToAction("Profil");
        }

        // GET: EditProfil
        public IActionResult EditProfil()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            return View(enseignant);
        }

        // POST: EditProfil
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

            // Mise à jour des champs
            enseignant.NomEnseignant = model.NomEnseignant;
            enseignant.Email = model.Email;
            enseignant.Telephone = model.Telephone;
            enseignant.DepartementAttache = model.DepartementAttache;
            enseignant.Filiere = model.Filiere;


            _context.SaveChanges();

            TempData["Success"] = "Profil mis à jour avec succès.";
            return RedirectToAction("Profil");
        }

        // ----------- List des sujets de ses étudiants -----------
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

            // Mapper vers SujetViewModel
            var sujets = enseignant.Stages
                .Where(s => s.Soutenance != null) // éviter null
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

        // ----------- Soutenances -----------
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






        // ----------- Liste des étudiants suivis par l’enseignant -----------
        [Route("Enseignant/Etudiants")]
        public IActionResult MesEtudiants()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            // Projection directe dans le ViewModel pour éviter les SqlNullValueException
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








        // GET: /Enseignant/Rapports
        public IActionResult Rapports()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            // Récupère l'enseignant
            var enseignant = _context.Enseignants
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (enseignant == null) return NotFound();

            // Liste des étudiants (ids) suivis par cet enseignant via les stages
            var etudiantIds = _context.Stages
                .Where(st => st.EnseignantId == enseignant.IdEnseignant)
                .Select(st => st.EtudiantId)
                .Distinct()
                .ToList();

            // Documents des étudiants encadrés par cet enseignant
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

            return View("Rapports", vm); // crée Views/Enseignant/Rapports.cshtml
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

            // Récupérer l'enseignant
            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            // Vérifier que le document appartient à un étudiant encadré
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
            // Vérifier que l'utilisateur est connecté
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            // Récupérer l'enseignant
            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null)
                return NotFound();

            // Récupérer le document
            var document = _context.Documents
                .Include(d => d.Etudiant)
                .FirstOrDefault(d => d.IdDocument == id);

            if (document == null)
                return NotFound("Document introuvable.");

            // Vérifier que c'est un rapport final
            if (document.TypeDocument.Trim().ToLower() != "rapportfinal")
                return BadRequest("Ce document n'est pas un rapport final.");

            // Vérifier que l'étudiant appartient à cet enseignant
            bool isEncadre = _context.Stages.Any(s => s.EtudiantId == document.EtudiantId
                                                      && s.EnseignantId == enseignant.IdEnseignant);
            if (!isEncadre)
                return Forbid("Vous n'avez pas la permission de télécharger ce document.");

            // Chemin du fichier sur le serveur
            var filePath = Path.Combine(_env.WebRootPath, "rapports", document.NomFichier);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Fichier introuvable sur le serveur.");

            // Retourner le fichier
            return PhysicalFile(filePath, "application/pdf", document.NomFichier);
        }


        // ----------- Notation des stages -----------
        [HttpGet]
        public IActionResult Notation(string filiere)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            var enseignant = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == userId);
            if (enseignant == null) return NotFound();

            // 👇 هادي تجيب جميع filières li kaynin f table Etudiants
            ViewBag.Filieres = _context.Etudiants
                .Select(e => e.Filiere)
                .Where(f => f != null && f != "") // نحيد null أو فارغ
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
                .Select(st => new NotationViewModel
                {
                    SoutenanceId = st.Soutenance.IdSoutenance,
                    EtudiantNom = st.Etudiant.NomComplet,
                    Filiere = st.Etudiant.Filiere,
                    SujetTitre = st.Soutenance.NomSujet,
                    Note = st.Soutenance.NoteFinale
                })
                .ToList();

            return View(model);
        }


        // ----------- Ajouter ou Modifier une note -----------
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult EnregistrerNote(int SoutenanceId, double Note, string filiere)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth", new { role = "Enseignant" });

            // vérifier si la soutenance existe
            var soutenance = _context.Soutenances
                .Include(s => s.Stage)
    .FirstOrDefault(s => s.IdSoutenance == SoutenanceId);

            if (soutenance != null)
            {
                soutenance.NoteFinale = Note;
                _context.SaveChanges();
                TempData["Success"] = "Note ajoutée avec succès.";
            }

            // Redirect avec filière sélectionnée
            return RedirectToAction("Notation", new { filiere });
        }

    }
}
