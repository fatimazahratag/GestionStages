using Microsoft.AspNetCore.Mvc;
using GestionStages.Data;
using GestionStages.Models;
using System.Linq;

namespace GestionStages.Controllers
{
    public class EnseignantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnseignantController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var enseignant = _context.Enseignants.FirstOrDefault(e => e.IdEnseignant == id);
            if (enseignant == null)
                return NotFound();

            return View(enseignant);
        }
    }
}
