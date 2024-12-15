using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmasuit.Data;
using Pharmasuit.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Linq;
using BC = BCrypt.Net.BCrypt;

namespace Pharmasuit.Controllers
{
    public class AccountController : Controller
    {
        private readonly PharmasuitContext _context;

        public AccountController(PharmasuitContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Accounts/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Account
                .FirstOrDefaultAsync(a => a.Name == username);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            // Verify the hashed password
            if (!BC.Verify(password, user.Password))
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            // Sign in the user
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Accounts/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(Account account, string password)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists
                var existingUser = await _context.Account
                    .FirstOrDefaultAsync(a => a.Name == account.Name);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Username already exists");
                    return View(account);
                }

                // Hash the password before storing
                account.Password = BC.HashPassword(password);

                _context.Account.Add(account);
                await _context.SaveChangesAsync();

                // Create authentication claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, account.Name),
            new Claim(ClaimTypes.Role, account.Role),
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                };

                // Sign in the user
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect to Home page after successful registration and login
                return RedirectToAction("Index", "Home");
            }

            // If we get here, something went wrong
            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}