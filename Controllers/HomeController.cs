using Microsoft.AspNetCore.Mvc;
using Pharmasuit.Data;
using Pharmasuit.Models;
using System.Diagnostics;
using System.Linq;

namespace Pharmasuit.Controllers
{
    public class HomeController : Controller
    {
        private readonly PharmasuitContext _context;

        public HomeController(PharmasuitContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            var products = _context.Products.ToList(); // Get all products from the database
            return View(products); // Pass products to the view
        }
    }

}
