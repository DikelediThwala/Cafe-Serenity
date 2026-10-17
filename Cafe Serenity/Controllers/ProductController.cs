using Coffee_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coffee_Shop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    return RedirectToAction("ProductList");
        //}

        [HttpGet]
        public IActionResult AddCoffee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCoffee(CoffeeProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            byte[] imageData = null;

            if (model.Image != null && model.Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.Image.CopyToAsync(ms);
                imageData = ms.ToArray();
            }

            var coffeeProduct = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                Quantity = model.Quantity,
                Category = model.Category,
                Image = imageData,
            };


            _context.Products.Add(coffeeProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProductList");
        }

        public async Task<IActionResult> ProductList()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            var viewModel = new CoffeeProductViewModel
            {
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Category = product.Category,
                ExistingImage = product.Image
            };

            ViewBag.ProductId = product.ProductId;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CoffeeProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var existingProduct = await _context.Products.FindAsync(id);

                if (existingProduct == null)
                    return NotFound();

                model.ExistingImage = existingProduct.Image;
                ViewBag.ProductId = id;

                return View(model);
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            product.Category = model.Category;

            // Only replace the saved image when a new image was uploaded.
            if (model.Image != null && model.Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.Image.CopyToAsync(ms);
                product.Image = ms.ToArray();
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product updated successfully.";

            return RedirectToAction(nameof(ProductList));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product deleted successfully.";

            return RedirectToAction(nameof(ProductList));
        }

    }
}
