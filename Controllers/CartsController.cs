using Microsoft.AspNetCore.Mvc;
using Pharmasuit.Services;
using Pharmasuit.Data;
using Pharmasuit.Models;

namespace Pharmasuit.Controllers
{
    public class CartsController : Controller
    {
        private readonly CartService _cartService;
        private readonly PharmasuitContext _context;

        public CartsController(CartService cartService, PharmasuitContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public IActionResult Index()
        {
            var cartItems = _cartService.GetCartItems();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            try
            {
                _cartService.AddToCart(productId, quantity);
                return RedirectToAction("Index", "Products");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            _cartService.UpdateQuantity(productId, quantity);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cartItems = _cartService.GetCartItems();
            if (cartItems.Count == 0)
            {
                return RedirectToAction("Index", "Products");
            }

            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalPrice = _cartService.CalculateTotalPrice()
            };

            return View(checkoutViewModel);
        }
    }

    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}