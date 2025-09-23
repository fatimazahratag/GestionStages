using GestionStages.Data;
using GestionStages.Models;
using GestionStages.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SelectPdf;
using System.IO.Compression;


namespace GestionStages.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Admin" });

            var admin = _context.Admins.FirstOrDefault(a => a.UtilisateurId == userId);
            if (admin == null) return NotFound();

            ViewBag.CountEtudiants = _context.Etudiants.Count();
            ViewBag.CountEnseignants = _context.Enseignants.Count();
            ViewBag.CountDocuments = _context.Documents.Count();
            ViewBag.CountSoutenances = _context.Soutenances.Count();

            ViewBag.UpcomingDeadlines = _context.Documents
                .Include(d => d.Etudiant)
                .Where(d => d.TypeDocument == "Rapport Final" && d.DateLimiteRapportFinal >= DateTime.Today)
                .OrderBy(d => d.DateLimiteRapportFinal)
                .ToList();

            ViewBag.LateReports = _context.Documents
                .Where(d => d.TypeDocument == "Rapport Final" && d.DateDepot > d.DateLimiteRapportFinal)
                .Count();

            ViewBag.RecentDocuments = _context.Documents
                .Include(d => d.Etudiant)
                .OrderByDescending(d => d.DateDepot)
                .Take(5)
                .ToList();

            ViewBag.Etudiants1A = _context.Etudiants.Count(e => e.Niveau == "1ère année");
            ViewBag.Etudiants2A = _context.Etudiants.Count(e => e.Niveau == "2ème année");
            ViewBag.Etudiants3A = _context.Etudiants.Count(e => e.Niveau == "3ème année");


            ViewBag.DocEnAttente = _context.Documents.Count(d => d.Statut == "En attente");
            ViewBag.DocEnPreparation = _context.Documents.Count(d => d.Statut == "En préparation");
            ViewBag.DocPret = _context.Documents.Count(d => d.Statut == "Prêt");

            return View(admin);
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var admin = _context.Admins
                            .Include(a => a.Utilisateur)
                            .FirstOrDefault(a => a.UtilisateurId == userId);

                if (admin != null)
                {
                    ViewBag.AdminName = admin.NomComplet;
                }
            }
        }


        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Admin" });

            var admin = _context.Admins
                        .Include(a => a.Utilisateur)
                        .FirstOrDefault(a => a.UtilisateurId == userId);

            if (admin == null) return NotFound();
            return View(admin);
        }

        


        [HttpGet]
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth", new { role = "Admin" });

            var admin = _context.Admins
                        .Include(a => a.Utilisateur)
                        .FirstOrDefault(a => a.UtilisateurId == userId);

            if (admin == null) return NotFound();
            return View(admin);
        }

        [HttpPost]
        public IActionResult Edit(int IdAdmin, string NomComplet, string Email, string Telephone)
        {
            var admin = _context.Admins
                        .Include(a => a.Utilisateur)
                        .FirstOrDefault(a => a.IdAdmin == IdAdmin);

            if (admin == null) return NotFound();

            admin.NomComplet = NomComplet;
            admin.Email = Email;
            admin.Telephone = Telephone;

            if (admin.Utilisateur != null)
            {
                admin.Utilisateur.NomUtilisateur = Email; 
            }

            _context.SaveChanges();

            TempData["Success"] = "Profil mis à jour avec succès.";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult ChangePassword(int UtilisateurId, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Les mots de passe ne correspondent pas.";
                return RedirectToAction("Profile");
            }

            var user = _context.Utilisateurs.FirstOrDefault(u => u.IdUtilisateur == UtilisateurId);
            var admin = _context.Admins.FirstOrDefault(a => a.UtilisateurId == UtilisateurId);

            if (user == null || admin == null)
            {
                TempData["ErrorMessage"] = "Utilisateur introuvable.";
                return RedirectToAction("Profile");
            }

            user.MotDePasse = NewPassword;
            admin.Password = NewPassword;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Mot de passe changé avec succès.";
            return RedirectToAction("Profile");
        }

        public IActionResult EtudiantsList()
        {
            var students = _context.Etudiants.ToList();
            return View(students);  
        }

        public IActionResult EtudiantDetails(int id)
        {
            var student = _context.Etudiants.FirstOrDefault(e => e.IdEtudiant == id);
            if (student == null) return NotFound();
            return View("EtudiantDetails", student);  
        }

        [HttpGet]
        public IActionResult EditEtudiant(int id)
        {
            var student = _context.Etudiants.FirstOrDefault(e => e.IdEtudiant == id);
            if (student == null) return NotFound();
            return View(student); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEtudiant(int IdEtudiant, string NomComplet, string CNE, string Email, string Telephone, string Module, string Niveau)
        {
            var existing = _context.Etudiants.FirstOrDefault(e => e.IdEtudiant == IdEtudiant);
            if (existing == null) return NotFound();

            existing.NomComplet = NomComplet;
            existing.CNE = CNE;
            existing.Email = Email;
            existing.Telephone = Telephone;
            existing.Module = Module;
            existing.Niveau = Niveau;

            _context.SaveChanges();
            TempData["Success"] = "Étudiant modifié avec succès.";
            return RedirectToAction("EtudiantsList");
        }




        public IActionResult EnseignantsList()
        {
            var teachers = _context.Enseignants.ToList();
            return View(teachers);
        }

        [HttpGet]
        public IActionResult EditEnseignant(int id)
        {
            var enseignant = _context.Enseignants.FirstOrDefault(e => e.IdEnseignant == id);
            if (enseignant == null)
            {
                return NotFound();
            }
            return View(enseignant); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEnseignant(int IdEnseignant, string NomEnseignant, string Email, string Telephone, string DepartementAttache, string Filiere)
        {
            var enseignant = _context.Enseignants.FirstOrDefault(e => e.IdEnseignant == IdEnseignant);
            if (enseignant == null)
            {
                TempData["Error"] = "Enseignant introuvable.";
                return RedirectToAction("EnseignantsList");
            }

            enseignant.NomEnseignant = NomEnseignant;
            enseignant.Email = Email;
            enseignant.Telephone = Telephone;
            enseignant.DepartementAttache = DepartementAttache;
            enseignant.Filiere = Filiere;

            _context.SaveChanges();

            TempData["Success"] = "Enseignant modifié avec succès.";

            return RedirectToAction("EnseignantsList");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEnseignant(
    string NomEnseignant,
    string Email,
    string Telephone,
    string DepartementAttache,
    string Filiere)
        {
            if (string.IsNullOrWhiteSpace(NomEnseignant) || string.IsNullOrWhiteSpace(Email))
            {
                TempData["Error"] = "Veuillez remplir tous les champs obligatoires.";
                return RedirectToAction("EnseignantsList");
            }

            var existingUser = _context.Utilisateurs.FirstOrDefault(u => u.NomUtilisateur == Email);
            if (existingUser != null)
            {
                TempData["Error"] = "Un compte avec cet email existe déjà.";
                return RedirectToAction("EnseignantsList");
            }

            var utilisateur = new Utilisateur
            {
                NomUtilisateur = Email,
                MotDePasse = "123456",   
                Role = "Enseignant"
            };

            _context.Utilisateurs.Add(utilisateur);
            _context.SaveChanges(); 

            var enseignant = new Enseignant
            {
                NomEnseignant = NomEnseignant,
                Email = Email,
                Telephone = Telephone,
                DepartementAttache = DepartementAttache,
                Filiere = Filiere,
                UtilisateurId = utilisateur.IdUtilisateur
            };

            _context.Enseignants.Add(enseignant);
            _context.SaveChanges();

            TempData["Success"] = "Enseignant ajouté avec succès (compte utilisateur créé).";
            return RedirectToAction("EnseignantsList");
        }




       
        public IActionResult SoutenanceList()
        {
            var soutenances = _context.Soutenances
     .Include(s => s.Stage)
         .ThenInclude(st => st.Etudiant)
     .ToList();

            var enseignants = _context.Enseignants.ToList();
            var etudiants = _context.Etudiants.ToList();

            var vm = new SoutenanceViewModel
            {
                Soutenances = soutenances,
                Enseignants = enseignants,
                Etudiants = etudiants
            };


            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSoutenance(int IdSoutenance, DateTime? DateSoutenance, string Lieu, string Jury1, string Jury2)
        {
            var s = _context.Soutenances.FirstOrDefault(x => x.IdSoutenance == IdSoutenance);
            if (s == null) return NotFound();

            if (!DateSoutenance.HasValue)
            {
                TempData["Error"] = "Veuillez sélectionner une date.";
                return RedirectToAction("SoutenanceList");
            }

            var start = DateSoutenance.Value;
            var end = start.AddMinutes(30);

            var conflicts = _context.Soutenances
                .Where(x => x.IdSoutenance != IdSoutenance && x.DateSoutenance.HasValue)
                .Where(x =>
                    (x.DateSoutenance.Value < end && x.DateSoutenance.Value.AddMinutes(30) > start) &&
                    (x.Lieu == Lieu || x.Jury1 == Jury1 || x.Jury2 == Jury2)
                )
                .ToList();
            if (!string.IsNullOrWhiteSpace(Jury1) && Jury1 == Jury2)
            {
                TempData["Error"] = "Jury 1 et Jury 2 doivent être différents.";
                return RedirectToAction("SoutenanceList");
            }

            if (conflicts.Any())
            {
                TempData["Error"] = "Ce créneau horaire, jury ou salle est déjà pris.";
                return RedirectToAction("SoutenanceList");
            }

            s.DateSoutenance = DateSoutenance;
            s.Lieu = Lieu;
            s.Jury1 = Jury1;
            s.Jury2 = Jury2;

            _context.SaveChanges();
            TempData["Success"] = "Soutenance mise à jour avec succès.";
            return RedirectToAction("SoutenanceList");
        }


        [HttpGet]
        public IActionResult GenerateSoutenancePDF()
        {
            var soutenances = _context.Soutenances
                .Include(s => s.Stage)
                    .ThenInclude(st => st.Etudiant)
                .OrderBy(s => s.Stage.Etudiant.Niveau)
                .ThenBy(s => s.DateSoutenance)
                .ToList();

            var pdfHtml = "<h1>Planning des Soutenances</h1><table border='1'><tr>" +
                          "<th>Étudiant</th><th>Niveau</th><th>Date</th><th>Lieu</th><th>Jury 1</th><th>Jury 2</th></tr>";

            foreach (var s in soutenances)
            {
                pdfHtml += $"<tr>" +
                    $"<td>{s.Stage.Etudiant.NomComplet}</td>" +
                    $"<td>{s.Stage.Etudiant.Niveau}</td>" +
                    $"<td>{s.DateSoutenance?.ToString("dd/MM/yyyy HH:mm")}</td>" +
                    $"<td>{s.Lieu}</td>" +
                    $"<td>{s.Jury1}</td>" +
                    $"<td>{s.Jury2}</td>" +
                    $"</tr>";
            }

            pdfHtml += "</table>";

            var converter = new HtmlToPdf();
            var pdf = converter.ConvertHtmlString(pdfHtml);
            return File(pdf.Save(), "application/pdf", "Soutenances.pdf");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSoutenance(int IdEtudiant, DateTime? DateSoutenance, string Lieu, string Jury1, string Jury2)
        {
            if (!DateSoutenance.HasValue)
            {
                TempData["Error"] = "Veuillez sélectionner une date.";
                return RedirectToAction("SoutenanceList");
            }

            if (Jury1 == Jury2)
            {
                TempData["Error"] = "Jury 1 et Jury 2 doivent être différents.";
                return RedirectToAction("SoutenanceList");
            }

            var etudiant = _context.Etudiants.FirstOrDefault(e => e.IdEtudiant == IdEtudiant);
            if (etudiant == null)
            {
                TempData["Error"] = "Étudiant introuvable.";
                return RedirectToAction("SoutenanceList");
            }

            var stage = _context.Stages.FirstOrDefault(s => s.EtudiantId == IdEtudiant);
            if (stage == null)
            {
                stage = new Stage
                {
                    EtudiantId = IdEtudiant,
                    Description = "Stage temporaire pour test"
                };
                _context.Stages.Add(stage);
                _context.SaveChanges();
            }

            var existingSoutenance = _context.Soutenances.FirstOrDefault(s => s.StageId == stage.IdStage);
            if (existingSoutenance != null)
            {
                TempData["Error"] = "Une soutenance existe déjà pour cet étudiant.";
                return RedirectToAction("SoutenanceList");
            }

            var startTime = DateSoutenance.Value;
            var endTime = startTime.AddMinutes(30);

            bool conflit = _context.Soutenances.Any(s =>
                s.DateSoutenance.HasValue &&
                s.Lieu == Lieu &&
                ((startTime >= s.DateSoutenance && startTime < s.DateSoutenance.Value.AddMinutes(30)) ||
                 (endTime > s.DateSoutenance && endTime <= s.DateSoutenance.Value.AddMinutes(30))));

            if (conflit)
            {
                TempData["Error"] = "Un autre étudiant a déjà une soutenance dans cette salle à ce créneau.";
                return RedirectToAction("SoutenanceList");
            }

            bool juryConflit = _context.Soutenances.Any(s =>
                s.DateSoutenance.HasValue &&
                ((s.Jury1 == Jury1 || s.Jury2 == Jury1 || s.Jury1 == Jury2 || s.Jury2 == Jury2) &&
                ((startTime >= s.DateSoutenance && startTime < s.DateSoutenance.Value.AddMinutes(30)) ||
                 (endTime > s.DateSoutenance && endTime <= s.DateSoutenance.Value.AddMinutes(30))))
            );

            if (juryConflit)
            {
                TempData["Error"] = "Un des jurys est déjà occupé à ce créneau.";
                return RedirectToAction("SoutenanceList");
            }

            var soutenance = new Soutenance
            {
                StageId = stage.IdStage,
                DateSoutenance = DateSoutenance,
                Lieu = Lieu,
                Jury1 = Jury1,
                Jury2 = Jury2
            };

            _context.Soutenances.Add(soutenance);
            _context.SaveChanges();

            TempData["Success"] = "Soutenance ajoutée avec succès.";
            return RedirectToAction("SoutenanceList");
        }

        public IActionResult DemandesList()
        {
            var demandes = _context.Documents
                .Include(d => d.Etudiant)
                .Where(d => !(new[] { "Rapport Final", "Rapport" }.Contains(d.TypeDocument))
                            && d.Statut != "Soumis")
                .ToList();

            return View(demandes);
        }


       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangerStatut(int id, string statut)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.IdDocument == id);
            if (doc == null) return NotFound();

            doc.Statut = statut;
            _context.SaveChanges();

            TempData["Success"] = "Statut mis à jour avec succès.";
            return RedirectToAction("DemandesList");
        }

        [HttpGet]
        public IActionResult GenererDocument(int id)
        {
            var doc = _context.Documents
                .Include(d => d.Etudiant)
                .FirstOrDefault(d => d.IdDocument == id);

            if (doc == null) return NotFound();
            var etu = doc.Etudiant;
            if (doc.TypeDocument?.Trim().ToLower() == "rapport final")
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/rapports", doc.NomFichier);
                if (!System.IO.File.Exists(filePath))
                {
                    TempData["Error"] = "Fichier introuvable sur le serveur.";
                    return RedirectToAction("RapportsList");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf", doc.NomFichier);
            }

            var stage = _context.Stages
                .Include(s => s.Organisme)
                .Include(s => s.Enseignant)
                .FirstOrDefault(s => s.EtudiantId == doc.EtudiantId);


            var logoPath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logoencgo.jpg");
            var logoPath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logo2.jpg");

            var logoFileUrl1 = $"file:///{logoPath1.Replace("\\", "/")}";
            var logoFileUrl2 = $"file:///{logoPath2.Replace("\\", "/")}";

            var values = new Dictionary<string, string>
                {
                    { "NomComplet", doc.Etudiant.NomComplet },
                    { "CNE", doc.Etudiant.CNE },
                    { "Niveau", doc.Etudiant.Niveau },
                        { "Filiere", doc.Etudiant.Filiere },

                    { "NomOrganisme", stage?.Organisme?.NomOrganisme ?? "[Nom Organisme]" },
                { "Adresse", stage?.Organisme != null
                        ? $"{stage.Organisme.Adresse}, {stage.Organisme.Ville}"
                        : "[Adresse Organisme]" },
                    { "EncadrantUniv", stage?.Enseignant?.NomEnseignant ?? "[Encadrant Univ]" },
                { "DateDebut", stage != null ? stage.DateDebut.ToString("dd/MM/yyyy") : "[Date Début]" },
                { "DateFin", stage != null ? stage.DateFin.ToString("dd/MM/yyyy") : "[Date Fin]" },
                { "DateTime", DateTime.Now.ToString("dd/MM/yyyy") }, 
                { "Annee", DateTime.Now.Year.ToString() }, 
                        { "LogoPath", logoFileUrl1 }, 
                    { "LogoPath2", logoFileUrl2 }  


                };


            string docType = doc.TypeDocument?.Trim().ToLower(); 
            string html = "";

            switch (docType)
            {
                case "convention de stage":
                    html = LoadTemplate("ConventionStage.html", values);
                    break;

                case "lettre d'acceptation entreprise":
                    html = LoadTemplate("LettreAcceptation.html", values);
                    break;

                case "attestation d'assurance":
                    html = LoadTemplate("AttestationAssurance.html", values);
                    break;

                case "fiche d'encadrement":
                    html = LoadTemplate("FicheEncadrement.html", values);
                    break;

                case "attestation de scolarité":
                    html = LoadTemplate("AttestationScolarite.html", values);
                    break;

                default:
                    html = $"<p>Pas de template défini pour {doc.TypeDocument}</p>";
                    break;
            
        }

        HtmlToPdf converter = new HtmlToPdf();
            PdfDocument pdf = converter.ConvertHtmlString(html);

            return File(pdf.Save(), "application/pdf", $"{doc.TypeDocument}_{etu.NomComplet}.pdf");
        }



       

        [HttpGet]
        public IActionResult GenererDossierStage(int etudiantId)
        {
            var etu = _context.Etudiants.FirstOrDefault(e => e.IdEtudiant == etudiantId);
            if (etu == null) return NotFound();

            var stage = _context.Stages
                .Include(s => s.Organisme)
                .Include(s => s.Enseignant)
                .FirstOrDefault(s => s.EtudiantId == etudiantId);

            var values = new Dictionary<string, string>
    {
        { "NomComplet", etu.NomComplet },
        { "CNE", etu.CNE },
        { "Niveau", etu.Niveau },
        { "Filiere", etu.Filiere },
        { "Module", etu.Module },
        { "NomEnseignant", stage?.Enseignant?.NomEnseignant ?? "[Nom Enseignant]" },
        { "Email", stage?.Enseignant?.Email ?? "[Email]" },
        { "DateDebut", stage?.DateDebut.ToString("dd/MM/yyyy") ?? "[DateDébut]" },
        { "DateFin", stage?.DateFin.ToString("dd/MM/yyyy") ?? "[DateFin]" },
        { "NomOrganisme", stage?.Organisme?.NomOrganisme ?? "[Nom Organisme]" },
        { "Adresse", stage?.Organisme != null ? $"{stage.Organisme.Adresse}, {stage.Organisme.Ville}" : "[Adresse]" },
        { "EncadrantUniv", stage?.Enseignant?.NomEnseignant ?? "[Encadrant Univ]" },
        { "EmailEncadrant", stage?.Enseignant?.Email ?? "[Email]" },
        { "NomEntreprise", stage?.Organisme?.NomOrganisme ?? "[Nom Entreprise]" },
        { "AdresseEntreprise", stage?.Organisme != null ? $"{stage.Organisme.Adresse}, {stage.Organisme.Ville}" : "[Adresse Entreprise]" },
        { "Ville", stage?.Organisme?.Ville ?? "[Ville]" },
        { "DateTime", DateTime.Now.ToString("dd/MM/yyyy") },
        { "Annee", DateTime.Now.Year.ToString() },
        { "LogoPath", Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logoencgo.jpg") },
        { "LogoPath2", Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logo2.jpg") },
        { "NomResponsable", "[Nom Responsable]" },
        { "EncadrantEntreprise", "[Nom Encadrant Entreprise]" },
        { "FonctionEncadrantEntreprise", "[Fonction]" }
    };

            var templates = new Dictionary<string, string>
    {
        { "Convention de stage", "ConventionStage.html" },
        { "Fiche d'encadrement", "FicheEncadrement.html" },
        { "Lettre d'acceptation entreprise", "LettreAcceptation.html" },
        { "Attestation d'assurance", "AttestationAssurance.html" }
    };

            var tempPath = Path.Combine(Path.GetTempPath(), $"DossierStage_{etu.IdEtudiant}_{DateTime.Now:yyyyMMddHHmmss}");
            Directory.CreateDirectory(tempPath);

            HtmlToPdf converter = new HtmlToPdf();

            foreach (var tpl in templates)
            {
                string html = LoadTemplate(tpl.Value, values);
                PdfDocument pdf = converter.ConvertHtmlString(html);
                string fileName = Path.Combine(tempPath, $"{tpl.Key}_{etu.NomComplet}.pdf");
                pdf.Save(fileName);
            }

            string zipPath = Path.Combine(Path.GetTempPath(), $"DossierStage_{etu.IdEtudiant}_{DateTime.Now:yyyyMMddHHmmss}.zip");
            ZipFile.CreateFromDirectory(tempPath, zipPath);

            Directory.Delete(tempPath, true);

            byte[] fileBytes = System.IO.File.ReadAllBytes(zipPath);
            return File(fileBytes, "application/zip", $"DossierStage_{etu.NomComplet}.zip");
        }




        private string LoadTemplate(string fileName, Dictionary<string, string> values)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Admin", "Templates", "Documents", fileName);

            if (!System.IO.File.Exists(path))
            {
                throw new FileNotFoundException($"Template introuvable: {path}");
            }

            string content = System.IO.File.ReadAllText(path);

            foreach (var kvp in values)
                content = content.Replace("{{" + kvp.Key + "}}", kvp.Value ?? "");

            return content;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SupprimerDocument(int id)
        {
            var doc = _context.Documents.Find(id);
            if (doc != null)
            {
                _context.Documents.Remove(doc);
                _context.SaveChanges();
            }
            return RedirectToAction("DemandesList");
        }






        public IActionResult RapportsList()
        {
            var rapports = _context.Documents
                            .Include(r => r.Etudiant)
                            .Where(r => r.TypeDocument != null && r.TypeDocument.Trim().ToLower() == "rapport final")
                            .ToList();

            return View(rapports);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetDeadline(int idDocument, DateTime deadline)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.IdDocument == idDocument);
            if (doc != null)
            {
                doc.DateLimiteRapportFinal = deadline;
                _context.SaveChanges();
            }
            return RedirectToAction("RapportsList"); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetGlobalDeadline(DateTime deadline)
        {
            var rapports = _context.Documents
                            .Where(d => d.TypeDocument == "Rapport Final")
                            .ToList();

            foreach (var doc in rapports)
            {
                doc.DateLimiteRapportFinal = deadline;
            }

            _context.SaveChanges();

            return RedirectToAction("RapportsList");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangerStatutRapport(int id, string statut)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.IdDocument == id);
            if (doc == null) return NotFound();

            if (doc.TypeDocument != "Rapport Final")
            {
                TempData["Error"] = "Ce document n'est pas un rapport final.";
                return RedirectToAction("RapportsList");
            }

            doc.Statut = statut;
            _context.SaveChanges();

            TempData["Success"] = "Statut du rapport mis à jour.";
            return RedirectToAction("RapportsList");
        }
        [HttpPost]
        public IActionResult DefinirDateLimiteRapport(DateTime dateLimite)
        {
            var rapports = _context.Documents
                .Where(d => d.TypeDocument == "Rapport Final")
                .ToList();

            foreach (var r in rapports)
            {
                r.DateLimiteRapportFinal = dateLimite;
            }

            _context.SaveChanges();

            TempData["Success"] = "✅ Date limite définie avec succès.";
            return RedirectToAction("RapportsList"); 
        }

        [HttpGet]
        public IActionResult TelechargerRapport(int id)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.IdDocument == id);
            if (doc == null) return NotFound();

            if (doc.TypeDocument.Trim().ToLower() != "rapportfinal")
            {
                TempData["Error"] = "Ce document n'est pas un rapport final.";
                return RedirectToAction("RapportsList");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", doc.NomFichier);
            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "Fichier introuvable sur le serveur.";
                return RedirectToAction("RapportsList");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", doc.NomFichier);
        }



       
        [HttpGet]
        public IActionResult SujetsList()
        {
            var sujets = _context.Soutenances
                .Include(s => s.Stage)
                    .ThenInclude(st => st.Etudiant)
                .Include(s => s.Stage)
                    .ThenInclude(st => st.Enseignant)
                .Select(s => new SujetViewModel
                {
                    IdSoutenance = s.IdSoutenance,
                    NomEtudiant = s.Stage.Etudiant.NomComplet,
                    NomEncadrant = s.Stage.Enseignant != null ? s.Stage.Enseignant.NomEnseignant : "Non affecté",
                    NomSujet = s.NomSujet ?? "",
                    DescriptionSujet = s.DescriptionSujet ?? "",
                    StatutSujet = s.StatutSujet ?? "En attente",
                    Note = s.NoteFinale.HasValue ? (decimal?)s.NoteFinale.Value : null
                })
                .ToList();

            ViewBag.Enseignants = _context.Enseignants
                .Include(e => e.Stages)
                .ToList();

            return View(sujets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AffecterEncadrant(int idSoutenance, int idEnseignant)
        {
            var soutenance = _context.Soutenances
                .Include(s => s.Stage)
                    .ThenInclude(st => st.Etudiant) 
                .FirstOrDefault(s => s.IdSoutenance == idSoutenance);

            if (soutenance == null || soutenance.Stage == null || soutenance.Stage.Etudiant == null)
            {
                TempData["Error"] = "Soutenance ou Stage non trouvé.";
                return RedirectToAction("SujetsList");
            }

            var enseignant = _context.Enseignants
                .Include(e => e.Stages)
                .FirstOrDefault(e => e.IdEnseignant == idEnseignant);

            if (enseignant == null)
            {
                TempData["Error"] = "Enseignant non trouvé.";
                return RedirectToAction("SujetsList");
            }

            int nbEtudiants = enseignant.Stages.Count;
            if (nbEtudiants >= 4)
            {
                TempData["Error"] = $"L'enseignant {enseignant.NomEnseignant} a déjà 4 étudiants.";
                return RedirectToAction("SujetsList");
            }

            soutenance.Stage.EnseignantId = idEnseignant;
            _context.SaveChanges();

            TempData["Success"] = $"✅ {enseignant.NomEnseignant} a été affecté à {soutenance.Stage.Etudiant.NomComplet}.";
            return RedirectToAction("SujetsList");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SupprimerSujet(int idSoutenance)
        {
            var s = _context.Soutenances.Find(idSoutenance);
            if (s != null)
            {
                _context.Soutenances.Remove(s);
                _context.SaveChanges();
                TempData["Success"] = "✅ Sujet supprimé avec succès.";
            }
            return RedirectToAction("SujetsList");
        }

        [HttpPost]
        public IActionResult ModifierNote(int idSoutenance, decimal? NoteFinale)
        {
            var soutenance = _context.Soutenances
                                .FirstOrDefault(s => s.IdSoutenance == idSoutenance);

            if (soutenance == null)
                return Json(new { success = false });

            soutenance.NoteFinale = NoteFinale.HasValue ? (double?)NoteFinale.Value : null;

            if (NoteFinale.HasValue)
                soutenance.StatutSujet = NoteFinale.Value < 12 ? "Refusé" : "Validé";
            else
                soutenance.StatutSujet = "En attente";

            _context.SaveChanges();

            return Json(new { success = true });
        }




    }
}
