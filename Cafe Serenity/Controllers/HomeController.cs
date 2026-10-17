using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coffee_Shop.Models;
using Microsoft.EntityFrameworkCore;

namespace Coffee_Shop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }


    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Menu()
    {
        var products = _context.Products.ToList();
        return View(products);
    }

}
