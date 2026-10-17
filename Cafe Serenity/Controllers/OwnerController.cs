
using Microsoft.AspNetCore.Mvc;
using Coffee_Shop.Models;

namespace Coffee_Shop.Controllers
{
    public class OwnerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private const long MaxMenuSize = 10 * 1024 * 1024; // 10 MB

        public OwnerController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private string GetMenuPath()
        {
            return Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "menus",
                "current-menu.pdf");
        }

        public IActionResult Index()
        {
            ViewBag.HasMenu = System.IO.File.Exists(GetMenuPath());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMenu(IFormFile? menuFile)
        {
            if (menuFile == null || menuFile.Length == 0)
            {
                TempData["MenuError"] = "Please select a PDF menu to upload.";
                return RedirectToAction(nameof(Index));
            }

            if (menuFile.Length > MaxMenuSize)
            {
                TempData["MenuError"] = "The PDF must be 10 MB or smaller.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.Equals(
                    Path.GetExtension(menuFile.FileName),
                    ".pdf",
                    StringComparison.OrdinalIgnoreCase))
            {
                TempData["MenuError"] = "Only PDF files are allowed.";
                return RedirectToAction(nameof(Index));
            }

            // Check the file signature, not just its extension.
            var signature = new byte[5];

            await using (var input = menuFile.OpenReadStream())
            {
                var bytesRead = await input.ReadAsync(
                    signature.AsMemory(0, signature.Length));

                if (bytesRead != 5 ||
                    System.Text.Encoding.ASCII.GetString(signature) != "%PDF-")
                {
                    TempData["MenuError"] =
                        "This file does not appear to be a valid PDF.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var menuPath = GetMenuPath();
            var menuDirectory = Path.GetDirectoryName(menuPath)!;

            Directory.CreateDirectory(menuDirectory);

            var temporaryPath = Path.Combine(
                menuDirectory,
                $"{Guid.NewGuid():N}.tmp");

            try
            {
                await using (var output = new FileStream(
                    temporaryPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    await menuFile.CopyToAsync(output);
                }

                // Replace the old menu only after the new file is saved.
                System.IO.File.Move(temporaryPath, menuPath, true);

                TempData["MenuSuccess"] =
                    "Your PDF menu has been uploaded successfully.";
            }
            catch
            {
                if (System.IO.File.Exists(temporaryPath))
                {
                    System.IO.File.Delete(temporaryPath);
                }

                TempData["MenuError"] =
                    "The menu could not be uploaded. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult PreviewMenu()
        {
            var menuPath = GetMenuPath();

            if (!System.IO.File.Exists(menuPath))
            {
                return NotFound("No PDF menu has been uploaded yet.");
            }

            return PhysicalFile(menuPath, "application/pdf");
        }

        [HttpGet]
        public IActionResult DownloadMenu()
        {
            var menuPath = GetMenuPath();

            if (!System.IO.File.Exists(menuPath))
            {
                return NotFound("No PDF menu has been uploaded yet.");
            }

            return PhysicalFile(
                menuPath,
                "application/pdf",
                "Cafe-Serenity-Menu.pdf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveMenu()
        {
            var menuPath = GetMenuPath();

            if (System.IO.File.Exists(menuPath))
            {
                System.IO.File.Delete(menuPath);
                TempData["MenuSuccess"] = "The PDF menu has been removed.";
            }
            else
            {
                TempData["MenuError"] = "There is no menu to remove.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}