using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Coffee_Shop.Models;
public partial class CoffeeProductViewModel
{
    [Required]
    public string ProductName { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [Range(0.01, 9999)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    public IFormFile? Image { get; set; }
    public byte[]? ExistingImage { get; set; }
    [Required]
    public string Category { get; set; }
}
