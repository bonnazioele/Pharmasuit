using Microsoft.AspNetCore.Mvc;
using Pharmasuit.Data;
using Pharmasuit.Models;
using System.Linq;

namespace Pharmasuit.Controllers
{
    public class AccountController : Controller
    {
        private readonly PharmasuitContext _context;

        public AccountController(PharmasuitContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Account.FirstOrDefault(a => a.Name == username && a.Password == password);
            if (user == null) return Unauthorized();

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Register(Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Account.Add(account);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(account);
        }
    }
}
