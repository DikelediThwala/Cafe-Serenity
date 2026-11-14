
using Coffee_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Coffee_Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            var cart = HttpContext.Session
                .GetObjectFromJson<List<Product>>("Cart")
                ?? new List<Product>();

            return View(cart);
        }

        // Adds an item without leaving the menu.
        public IActionResult AddToCartAjax(int id)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session
                .GetObjectFromJson<List<Product>>("Cart")
                ?? new List<Product>();

            cart.Add(product);

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Json(new
            {
                success = true,
                count = cart.Count
            });
        }

        // Keeps the original AddToCart route available.
        public IActionResult AddToCart(int id)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session
                .GetObjectFromJson<List<Product>>("Cart")
                ?? new List<Product>();

            cart.Add(product);

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Menu", "Home");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session
                .GetObjectFromJson<List<Product>>("Cart")
                ?? new List<Product>();

            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IncreaseQuantity(int id)
        {
            var cart = HttpContext.Session
                .GetObjectFromJson<List<Product>>("Cart")
                ?? new List<Product>();

            var product = cart.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                product = _context.Products.FirstOrDefault(p => p.ProductId == id);

                if (product == null)
                    return NotFound();

                cart.Add(product);
            }
            else
            {
                cart.Add(product);
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DecreaseQuantity(int id)
        {
            var cart = HttpContext.Session
                .GetObjectFromJson<List<Product>>("Cart")
                ?? new List<Product>();

            var product = cart.FirstOrDefault(p => p.ProductId == id);

            if (product != null)
            {
                cart.Remove(product);
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveItem(int id)
        {
            var cart = HttpContext.Session
                .GetObjectFromJson<List<Product>>("Cart")
                ?? new List<Product>();

            cart.RemoveAll(p => p.ProductId == id);

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction(nameof(Index));
        }

    }
}