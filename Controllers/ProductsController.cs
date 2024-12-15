using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmasuit.Data;
using Pharmasuit.Services;
using Pharmasuit.Models;

namespace Pharmasuit.Controllers
{
    public class ProductsController : Controller
    {
        private readonly PharmasuitContext _context;
        private readonly CartService _cartService;

        public ProductsController(PharmasuitContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
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