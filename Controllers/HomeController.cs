using Microsoft.AspNetCore.Mvc;
using Pharmasuit.Data;
using Pharmasuit.Services;

namespace Pharmasuit.Controllers
{
    public class HomeController : Controller
    {
        private readonly PharmasuitContext _context;
        private readonly CartService _cartService;

        public HomeController(PharmasuitContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            // Fetch featured or recommended products
            var featuredProducts = _context.Products.Take(3).ToList();
            return View(featuredProducts);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            try
            {
                _cartService.AddToCart(productId, quantity);
                TempData["SuccessMessage"] = "Product added to cart successfully!";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}