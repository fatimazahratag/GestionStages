using GestionStages.Data;
using GestionStages.Models;
using GestionStages.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionStages.Controllers
{

    public class EtudiantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EtudiantController> _logger;

        public EtudiantController(ApplicationDbContext context, ILogger<EtudiantController> logger)
        {
            _context = context;
            _logger = logger;

        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var etudiant = _context.Etudiants
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (etudiant == null)
                return NotFound("Étudiant non trouvé");

            // 🎓 Filière
            ViewBag.Filiere = etudiant.Filiere ?? "Non définie";

            // 📁 Nombre de stages
            ViewBag.TotalStages = _context.Stages
                .Count(s => s.EtudiantId == etudiant.IdEtudiant);

            // 📄 Dernière demande de document
            var derniereDemandeDoc = _context.Documents
                .Where(d => d.EtudiantId == etudiant.IdEtudiant)
                .OrderByDescending(d => d.DateDepot)
                .FirstOrDefault();

            ViewBag.DerniereDemandeDoc = derniereDemandeDoc != null
                ? $"{derniereDemandeDoc.TypeDocument}"
                : "Aucune demande déposée";

            // 📝 Soutenance
            var soutenance = _context.Soutenances
                .Include(s => s.Stage)
                .Where(s => s.Stage.EtudiantId == etudiant.IdEtudiant)
                .OrderByDescending(s => s.DateSoutenance)
                .FirstOrDefault();

            if (soutenance != null)
            {
                // Note finale
                ViewBag.NoteFinale = soutenance.NoteFinale.HasValue
                    ? string.Format("{0:0.0}", soutenance.NoteFinale.Value)
                    : "Pas encore attribuée";

                // Date & lieu
                if (soutenance.NoteFinale.HasValue)
                {
                    ViewBag.SoutenanceInfo = $"Soutenue le {soutenance.DateSoutenance?.ToString("dd/MM/yyyy")} à {soutenance.Lieu ?? "Lieu non défini"}";
                }
                else
                {
                    ViewBag.SoutenanceInfo = soutenance.DateSoutenance.HasValue
                        ? $"Prévue le {soutenance.DateSoutenance.Value:dd/MM/yyyy} à {soutenance.Lieu ?? "Lieu non défini"}"
                        : "Pas encore planifiée";
                }

                // ✅ Statut Sujet
                ViewBag.StatutSujet = string.IsNullOrEmpty(soutenance.StatutSujet)
                    ? "En attente"
                    : soutenance.StatutSujet;
            }
            else
            {
                ViewBag.NoteFinale = "Pas encore attribuée";
                ViewBag.SoutenanceInfo = "Aucune soutenance enregistrée";
                ViewBag.StatutSujet = "Non défini";
            }

            return View(etudiant);
        }




        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var etudiant = _context.Etudiants
                .Include(e => e.Stages)
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (etudiant == null)
                return NotFound("Étudiant non trouvé");

            return View(etudiant);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var etudiant = _context.Etudiants
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (etudiant == null) return NotFound();

            var model = new EditProfileViewModel
            {
                NomComplet = etudiant.NomComplet,
                Email = etudiant.Email,
                Telephone = etudiant.Telephone,
                Filiere = etudiant.Filiere,
                Niveau = etudiant.Niveau,
                Module = etudiant.Module
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var etudiant = _context.Etudiants
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (etudiant == null) return NotFound();

            // Mise à jour
            etudiant.NomComplet = model.NomComplet;
            etudiant.Email = model.Email;
            etudiant.Telephone = model.Telephone;
            etudiant.Filiere = model.Filiere;
            etudiant.Niveau = model.Niveau;
            etudiant.Module = model.Module;

            _context.SaveChanges();

            TempData["Success"] = "Profil mis à jour avec succès ✅";
            return RedirectToAction("Profile");
        }
        // ✅ Helper: récupérer l'étudiant connecté
        private Etudiant GetCurrentEtudiant()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return null;

            return _context.Etudiants.FirstOrDefault(e => e.UtilisateurId == userId);
        }

        // ✅ MesStages
        [HttpGet]
        public IActionResult MesStages()
        {
            var etudiant = GetCurrentEtudiant();
            if (etudiant == null) return RedirectToAction("Login", "Auth");

            var stages = _context.Stages
                .Include(s => s.Enseignant)
                .Include(s => s.Organisme)
                .Where(s => s.EtudiantId == etudiant.IdEtudiant)
                .OrderByDescending(s => s.DateDebut) // ✅ le plus récent d’abord
                .Select(s => new MesStagesViewModel
                {
                    IdStage = s.IdStage,
                    TypeStage = s.TypeStage,
                    Theme = s.Theme,
                    DateDebut = s.DateDebut,
                    DateFin = s.DateFin,
                    ConfirmationEtudiant = s.ConfirmationEtudiant,
                    NomEnseignant = s.Enseignant.NomEnseignant,
                    NomOrganisme = s.Organisme.NomOrganisme,
                    VilleOrganisme = s.Organisme.Ville,

                    // On calcule l'état ici
                    Etat = !s.ConfirmationEtudiant ? "En attente de confirmation"
                           : DateTime.Now > s.DateFin ? "Terminé"
                           : "En cours"
                })
                .ToList();

            return View(stages);
        }



        [HttpGet]
        public IActionResult AjouterDemandeStage()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var etudiant = _context.Etudiants
                .FirstOrDefault(e => e.UtilisateurId == userId);

            if (etudiant == null) return NotFound();

            // Récupérer la liste des enseignants
            var enseignants = _context.Enseignants
                .Select(e => new { e.IdEnseignant, e.NomEnseignant })
                .ToList();

            ViewBag.EnseignantsList = new SelectList(enseignants, "IdEnseignant", "NomEnseignant");

            return View();
        }


        [HttpPost]
        public IActionResult AjouterDemandeStage(StageDemandeViewModel model)
        {
            var etudiant = GetCurrentEtudiant();
            if (etudiant == null) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                // Recharger la liste enseignants en cas d'erreur de validation
                var enseignants = _context.Enseignants
                    .Select(e => new { e.IdEnseignant, e.NomEnseignant })
                    .ToList();
                ViewBag.EnseignantsList = new SelectList(enseignants, "IdEnseignant", "NomEnseignant");

                return View(model);
            }

            var organisme = new Organisme
            {
                NomOrganisme = model.Entreprise,
                Departement = model.DepartementOrganisme,
                SecteurActivite = model.SecteurActiviteOrganisme,
                Adresse = model.AdresseOrganisme,
                Ville = model.VilleOrganisme,
                NomResponsable = model.NomResponsableOrganisme,
                FonctionResponsable = model.FonctionResponsableOrganisme,
                EmailResponsable = model.EmailResponsableOrganisme,
                TelephoneResponsable = model.TelephoneResponsableOrganisme
            };

            _context.Organismes.Add(organisme);
            _context.SaveChanges();

            var stage = new Stage
            {
                TypeStage = model.TypeStage,
                Theme = model.Sujet,
                DateDebut = model.DateDebut,
                DateFin = model.DateFin,
                AutorisationTraitement = model.AutorisationTraitement,
                ConfirmationEtudiant = false,
                EtudiantId = etudiant.IdEtudiant,
                OrganismeId = organisme.IdOrganisme,
                EnseignantId = model.EnseignantId, // ✅ add this

                NomSignature = etudiant.NomComplet ?? "N/A", // ✅ prevents NULL
                DateSoumission = DateTime.Now               // ✅ prevents NULL
            };


            _context.Stages.Add(stage);
            _context.SaveChanges();

            return RedirectToAction("MesStages");
        }


        [HttpGet]
        public async Task<IActionResult> MesDemandes()
        {
            var etudiant = GetCurrentEtudiant();
            if (etudiant == null) return RedirectToAction("Login", "Auth");

            var demandes = await _context.Documents
    .Where(d => d.EtudiantId == etudiant.IdEtudiant)
    .OrderByDescending(d => d.DateDepot)
    .Select(d => new DemandeViewModel
    {
        Id = d.IdDocument,
        TypeDocument = d.TypeDocument ?? "",
        DateDepot = d.DateDepot,
        Statut = d.Statut ?? "En attente",
        NomFichier = d.NomFichier ?? ""
    })
    .ToListAsync();


            var model = new MesDemandesViewModel
            {
                ListeDemandes = demandes
            };

            return View(model);
        }
        [HttpGet]
public async Task<IActionResult> GetDemandesStatus()
{
    var etudiant = GetCurrentEtudiant();
    if (etudiant == null) return Json(new { success = false });

    var demandes = await _context.Documents
        .Where(d => d.EtudiantId == etudiant.IdEtudiant)
        .Select(d => new
        {
            d.IdDocument,
            d.Statut
        })
        .ToListAsync();

    return Json(new { success = true, demandes });
}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> MesDemandes(MesDemandesViewModel model)
        //{
        //    var etudiant = GetCurrentEtudiant();
        //    if (etudiant == null) return RedirectToAction("Login", "Auth");

        //    // Vérifier que TypeDocument est rempli
        //    if (string.IsNullOrEmpty(model.NouvelleDemande?.TypeDocument))
        //    {
        //        TempData["Error"] = "Veuillez sélectionner un type de document.";

        //        model.ListeDemandes = await _context.Documents
        //            .Where(d => d.EtudiantId == etudiant.IdEtudiant)
        //            .Select(d => new DemandeViewModel
        //            {
        //                Id = d.IdDocument,
        //                TypeDocument = d.TypeDocument ?? "",
        //                DateDepot = d.DateDepot,
        //                Statut = d.Statut ?? "En attente",
        //                NomFichier = d.NomFichier ?? ""
        //            })
        //            .ToListAsync();

        //        return View(model);
        //    }

        //    var demande = new Document
        //    {
        //        TypeDocument = model.NouvelleDemande.TypeDocument,
        //        DateDepot = DateTime.Now,
        //        Statut = "En attente",
        //        EtudiantId = etudiant.IdEtudiant
        //    };

        //    _context.Documents.Add(demande);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("MesDemandes");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MesDemandes(MesDemandesViewModel model)
        {
            var etudiant = GetCurrentEtudiant();
            if (etudiant == null) return RedirectToAction("Login", "Auth");

            if (string.IsNullOrEmpty(model.NouvelleDemande?.TypeDocument))
            {
                TempData["Error"] = "Veuillez sélectionner un type de document.";

                model.ListeDemandes = await _context.Documents
                    .Where(d => d.EtudiantId == etudiant.IdEtudiant)
                    .Select(d => new DemandeViewModel
                    {
                        Id = d.IdDocument,
                        TypeDocument = d.TypeDocument ?? "",
                        DateDepot = d.DateDepot,
                        Statut = d.Statut ?? "En attente",
                        NomFichier = d.NomFichier ?? ""
                    })
                    .ToListAsync();

                return View(model);
            }

            // ====== Handle "Dossier de stage" ======
            //    if (model.NouvelleDemande.TypeDocument == "Dossier de stage")
            //    {
            //        var dossierDocs = new[] {
            //    "Convention de stage",
            //    "Lettre d'acceptation entreprise",
            //    "Attestation d'assurance",
            //    "Fiche d'encadrement"
            //};

            //        foreach (var type in dossierDocs)
            //        {
            //            _context.Documents.Add(new Document
            //            {
            //                TypeDocument = type,
            //                DateDepot = DateTime.Now,
            //                Statut = "En attente",
            //                EtudiantId = etudiant.IdEtudiant
            //            });
            //        }
            //    }
            //    else
            //    {
            //        // Single document (e.g., Attestation de scolarité)
            //        _context.Documents.Add(new Document
            //        {
            //            TypeDocument = model.NouvelleDemande.TypeDocument,
            //            DateDepot = DateTime.Now,
            //            Statut = "En attente",
            //            EtudiantId = etudiant.IdEtudiant
            //        });
            //    }
            _context.Documents.Add(new Document
            {
                TypeDocument = model.NouvelleDemande.TypeDocument, // "Dossier de stage"
                DateDepot = DateTime.Now,
                Statut = "En attente",
                EtudiantId = etudiant.IdEtudiant
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("MesDemandes");
        }




        // GET: Rapport
        public async Task<IActionResult> Rapport()
        {
            var etudiant = GetCurrentEtudiant();
            if (etudiant == null) return RedirectToAction("Login", "Auth");

            // récupérer la date limite (la plus récente) depuis les documents
            var dateLimite = await _context.Documents
                .Where(d => d.TypeDocument == "Rapport Final" && d.DateLimiteRapportFinal != null)
                .Select(d => d.DateLimiteRapportFinal)
                .FirstOrDefaultAsync();

            var model = new RapportViewModel
            {
                Niveau = etudiant.Niveau,
                DateLimiteRapportFinal = dateLimite,
                ListeRapports = await _context.Documents
                    .Where(d => d.EtudiantId == etudiant.IdEtudiant && d.TypeDocument == "Rapport Final")
                    .Include(d => d.Etudiant)
                    .ToListAsync()
            };

            return View(model);
        }

        // POST: Rapport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rapport(RapportViewModel model)
        {
            var etudiant = GetCurrentEtudiant();
            if (etudiant == null) return RedirectToAction("Login", "Auth");

            if (model.Rapport == null || model.Rapport.Length == 0)
            {
                TempData["Error"] = "Veuillez sélectionner un fichier PDF.";
                return RedirectToAction("Rapport");
            }

            // Sauvegarde fichier dans wwwroot/rapports
            var fileName = $"{Guid.NewGuid()}_{model.Rapport.FileName}";
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/rapports", fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await model.Rapport.CopyToAsync(stream);
            }

            // Sauvegarde en base
            var doc = new Document
            {
                TypeDocument = "Rapport Final",
                DateDepot = DateTime.Now,
                Statut = "Soumis",
                NomFichier = fileName,
                EtudiantId = etudiant.IdEtudiant,
                DateLimiteRapportFinal = model.DateLimiteRapportFinal // ← ici
            };

            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rapport déposé avec succès ✅";
            return RedirectToAction("Rapport");
        }


        public async Task<IActionResult> Download(int id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return NotFound();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/rapports", doc.NomFichier);
            if (!System.IO.File.Exists(path)) return NotFound();

            // Peu importe le nom en DB → tu forces l'affichage
            return File(System.IO.File.ReadAllBytes(path), "application/pdf", "Rapport Final.pdf");
        }




        // GET: Télécharger rapport
        public IActionResult TelechargerRapport(int id)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.IdDocument == id);
            if (doc == null) return NotFound();

            var path = Path.Combine("wwwroot/rapports", doc.NomFichier);
            var mime = "application/pdf";
            return PhysicalFile(path, mime, doc.NomFichier);
        }


        // ✅ Sujets - GET
        [HttpGet]
        public IActionResult Sujets()
        {
            var etudiant = GetCurrentEtudiant();
            if (etudiant == null) return RedirectToAction("Login", "Auth");

            // Chercher le stage + encadrant
            var stage = _context.Stages
                .Include(st => st.Enseignant)
                .FirstOrDefault(st => st.EtudiantId == etudiant.IdEtudiant);

            if (stage == null)
            {
                TempData["Error"] = "Aucun stage trouvé pour vous.";
                return RedirectToAction("MesStages");
            }

            // Chercher la soutenance liée au stage
            var soutenance = _context.Soutenances
                .FirstOrDefault(s => s.StageId == stage.IdStage);

            var model = new SujetViewModel
            {
                NomEncadrant = stage.Enseignant?.NomEnseignant ?? "Non assigné",
                NomSujet = soutenance?.NomSujet,
                Description = soutenance?.DescriptionSujet,
                Note = (decimal?)soutenance?.NoteFinale
            };

            return View(model);
        }


        // ✅ Sujets - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AjouterSujet(SujetViewModel model)
        {
            var etudiant = GetCurrentEtudiant();
            if (etudiant == null)
            {
                _logger.LogWarning("POST AjouterSujet : utilisateur non connecté ou session expirée.");
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Validation error: {Error}", error.ErrorMessage);
                }

                // ⚡ Recharger le nom de l’encadrant même en cas d’erreur
                var stageReload = _context.Stages.Include(st => st.Enseignant)
                    .FirstOrDefault(st => st.EtudiantId == etudiant.IdEtudiant);
                model.NomEncadrant = stageReload?.Enseignant?.NomEnseignant ?? "Non assigné";

                return View("Sujets", model);
            }

            var stage = _context.Stages
                .Include(st => st.Enseignant)
                .FirstOrDefault(st => st.EtudiantId == etudiant.IdEtudiant);

            if (stage == null)
            {
                TempData["Error"] = "Aucun stage trouvé pour vous. Veuillez d'abord créer un stage.";
                return RedirectToAction("MesStages");
            }

            var soutenance = _context.Soutenances.FirstOrDefault(s => s.StageId == stage.IdStage);

            if (soutenance == null)
            {
                soutenance = new Soutenance
                {
                    StageId = stage.IdStage,
                    NomSujet = model.NomSujet,
                    DescriptionSujet = model.Description,
                    StatutSujet = "En attente",
                    DateSoutenance = DateTime.Now,
                    Lieu = "À définir",
                    Jury1 = "À définir",
                    Jury2 = "À définir"
                };

                _context.Soutenances.Add(soutenance);
            }

            else
            {
                soutenance.NomSujet = model.NomSujet;
                soutenance.DescriptionSujet = model.Description;
                soutenance.StatutSujet = "En attente";
            }

            _context.SaveChanges();

            TempData["Success"] = "✅ Sujet soumis avec succès !";
            return RedirectToAction("Sujets");
        }

//soutenance

        [HttpGet]
        public IActionResult MaSoutenance()
        {
            var etudiant = GetCurrentEtudiant();
            if (etudiant == null) return RedirectToAction("Login", "Auth");

            var soutenance = _context.Soutenances
                .Include(s => s.Stage)
                .ThenInclude(st => st.Etudiant)
                .FirstOrDefault(s => s.Stage.EtudiantId == etudiant.IdEtudiant);

            if (soutenance == null)
            {
                TempData["Error"] = "Aucune soutenance trouvée pour votre stage.";
                return RedirectToAction("MesStages");
            }

            var model = new SoutenanceEtudiantViewModel
            {
                NomSujet = soutenance.NomSujet,
                Description = soutenance.DescriptionSujet,
                DateSoutenance = soutenance.DateSoutenance,
                Lieu = soutenance.Lieu,
                Jury1 = soutenance.Jury1,
                Jury2 = soutenance.Jury2,
                NoteFinale = soutenance.NoteFinale
            };

            return View(model);
        }


        
    }
}
