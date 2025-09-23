using Microsoft.AspNetCore.Mvc;
using GestionStages.Data;
using GestionStages.Models;
using GestionStages.Models.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GestionStages.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string role)
        {
            ViewBag.Role = role;
            return View();
        }
       
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Role = model.Role;
                return View(model);
            }

            var user = _context.Utilisateurs.FirstOrDefault(u =>
                u.NomUtilisateur == model.NomUtilisateur &&
                u.MotDePasse == model.MotDePasse &&
                u.Role == model.Role);

            if (user == null)
            {
                ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect.");
                ViewBag.Role = model.Role;
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.IdUtilisateur);

            switch (user.Role)
            {
                case "Etudiant":
                    var etu = _context.Etudiants.FirstOrDefault(e => e.UtilisateurId == user.IdUtilisateur);
                    if (etu == null)
                    {
                        ModelState.AddModelError("", "Profil étudiant introuvable.");
                        ViewBag.Role = model.Role;
                        return View(model);
                    }
                    return RedirectToAction("Dashboard", "Etudiant");

                case "Enseignant":
                    var ens = _context.Enseignants.FirstOrDefault(e => e.UtilisateurId == user.IdUtilisateur);
                    if (ens == null)
                    {
                        ModelState.AddModelError("", "Profil enseignant introuvable.");
                        ViewBag.Role = model.Role;
                        return View(model);
                    }
                    return RedirectToAction("Dashboard", "Enseignant");

                case "Admin":
                    var adm = _context.Admins.FirstOrDefault(a => a.UtilisateurId == user.IdUtilisateur);
                    if (adm == null)
                    {
                        ModelState.AddModelError("", "Profil admin introuvable.");
                        ViewBag.Role = model.Role;
                        return View(model);
                    }
                    return RedirectToAction("Dashboard", "Admin");

                default:
                    ModelState.AddModelError("", "Rôle inconnu.");
                    ViewBag.Role = model.Role;
                    return View(model);
            }
        }

        [HttpGet]
        public IActionResult RegisterEtudiant()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterEtudiant(RegisterEtudiantViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_context.Utilisateurs.Any(u => u.NomUtilisateur == model.CNE))
            {
                ModelState.AddModelError("CNE", "Ce CNE est déjà utilisé.");
                return View(model);
            }

            var user = new Utilisateur
            {
                NomUtilisateur = model.CNE,
                MotDePasse = model.MotDePasse,
                Role = "Etudiant"
            };

            _context.Utilisateurs.Add(user);
            _context.SaveChanges();  

            var etudiant = new Etudiant
            {
                NomComplet = model.NomComplet,
                CNE = model.CNE,
                Email = model.Email,
                Telephone = model.Telephone,
                Filiere = model.Filiere,
                Niveau = model.Niveau,
                Module = model.Module,
                UtilisateurId = user.IdUtilisateur
            };

            _context.Etudiants.Add(etudiant);
            _context.SaveChanges();

            return RedirectToAction("Login", new { role = "Etudiant" });
        }

        [HttpGet]
        public IActionResult RegisterEnseignant()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEnseignant(RegisterEnseignantViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Utilisateurs.AnyAsync(u => u.NomUtilisateur == model.Email))
            {
                ModelState.AddModelError("Email", "Cet e-mail est déjà utilisé.");
                return View(model);
            }

            var user = new Utilisateur
            {
                NomUtilisateur = model.Email,
                MotDePasse = model.MotDePasse,
                Role = "Enseignant"
            };
            _context.Utilisateurs.Add(user);
            await _context.SaveChangesAsync();

            var enseignant = new Enseignant
            {
                NomEnseignant = model.NomEnseignant,
                Email = model.Email,
                Telephone = model.Telephone,
                DepartementAttache = model.DepartementAttache,
                Filiere = model.Filiere,
                UtilisateurId = user.IdUtilisateur
            };
            _context.Enseignants.Add(enseignant);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", new { role = "Enseignant" });
        }


        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(RegisterAdminViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.Utilisateurs.AnyAsync(u => u.NomUtilisateur == model.Email))
            {
                ModelState.AddModelError("Email", "Cet e-mail est déjà utilisé.");
                return View(model);
            }

            var user = new Utilisateur
            {
                NomUtilisateur = model.Email,  
                MotDePasse = model.MotDePasse,  
                Role = "Admin"
            };
            _context.Utilisateurs.Add(user);
            await _context.SaveChangesAsync();

            var admin = new Admin
            {
                NomComplet = model.NomComplet,
                Email = model.Email,
                Telephone = model.Telephone,
                Password = model.MotDePasse,
                UtilisateurId = user.IdUtilisateur
            };
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", new { role = "Admin" });
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
