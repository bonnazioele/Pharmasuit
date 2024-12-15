using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmasuit.Data;
using Pharmasuit.Models;
using Pharmasuit.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pharmasuit.Services;


namespace Pharmasuit.Controllers
{
    public class OrdersController : Controller
    {
        private readonly PharmasuitContext _context;
        private readonly CartService _cartService;

        public OrdersController(PharmasuitContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.Medicines)
                .ThenInclude(m => m.Product)
                .ToList();
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.Medicines)
                .ThenInclude(m => m.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            // Prepare any necessary data for the view
            var cartItems = _cartService.GetCartItems();

            if (cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }

            var orderViewModel = new OrderCreateViewModel
            {
                CartItems = cartItems,
                TotalPrice = cartItems.Sum(item => item.Product.Price * item.Quantity)
            };

            return View(orderViewModel);
        }

        // ViewModel to pass cart items to the order creation view
        public class OrderCreateViewModel
        {
            public List<CartItem> CartItems { get; set; }
            public decimal TotalPrice { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderCreateDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate products dropdown if model state is invalid
                ViewBag.Products = _context.Products
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    })
                    .ToList();
                return View(orderDto);
            }

            // Begin transaction to ensure data integrity
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Calculate total price
                decimal totalPrice = 0;
                var orderMedicines = new List<ProductOrder>();

                foreach (var item in orderDto.Medicines)
                {
                    var product = _context.Products.Find(item.ProductId);
                    if (product == null)
                    {
                        ModelState.AddModelError("", $"Product with ID {item.ProductId} not found.");
                        return View(orderDto);
                    }

                    // Check product availability
                    if (product.Stock < item.Quantity)
                    {
                        ModelState.AddModelError("", $"Insufficient stock for {product.Name}");
                        return View(orderDto);
                    }

                    // Reduce stock
                    product.Stock -= item.Quantity;

                    var orderMedicine = new ProductOrder
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Product = product
                    };

                    orderMedicines.Add(orderMedicine);
                    totalPrice += product.Price * item.Quantity;
                }

                // Create order
                var order = new Order
                {
                    CustomerName = orderDto.CustomerName,
                    Address = orderDto.Address,
                    Medicines = orderMedicines,
                    TotalPrice = totalPrice
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Commit transaction
                transaction.Commit();

                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of error
                transaction.Rollback();
                ModelState.AddModelError("", "An error occurred while processing your order.");
                return View(orderDto);
            }
        }
        [HttpPost]
        public IActionResult ConfirmOrder(string CustomerName, string Address)
        {
            var cartItems = _cartService.GetCartItems();

            if (cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                // Create Order logic similar to previous implementation
                var order = new Order
                {
                    CustomerName = CustomerName,
                    Address = Address,
                    TotalPrice = cartItems.Sum(item => item.Product.Price * item.Quantity),
                    Medicines = cartItems.Select(item => new ProductOrder
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Product = item.Product
                    }).ToList()
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Clear the cart after successful order
                _cartService.ClearCart();

                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("Details", new { id = order.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your order.";
                return RedirectToAction("Index", "Cart");
            }
        }
    }
}