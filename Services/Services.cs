using System.Collections.Generic;
using System.Linq;
using Pharmasuit.Data;
using Pharmasuit.Models;

namespace Pharmasuit.Services
{
    public class CartService
    {
        private readonly PharmasuitContext _context;
        private List<CartItem> _cartItems;

        public CartService(PharmasuitContext context)
        {
            _context = context;
            _cartItems = new List<CartItem>();
        }

        public void AddToCart(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
                throw new ArgumentException("Product not found");

            var existingCartItem = _cartItems.FirstOrDefault(c => c.ProductId == productId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Product = product,
                    Quantity = quantity
                });
            }
        }

        public void RemoveFromCart(int productId)
        {
            var cartItem = _cartItems.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                _cartItems.Remove(cartItem);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var cartItem = _cartItems.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
            }
        }

        public List<CartItem> GetCartItems()
        {
            return _cartItems;
        }

        public decimal CalculateTotalPrice()
        {
            return _cartItems.Sum(item => item.Product.Price * item.Quantity);
        }

        public void ClearCart()
        {
            _cartItems.Clear();
        }
    }
}