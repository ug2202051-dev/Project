using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShopSana.Models;
using ShopSana.Models.ViewModels;
using ShopSana.Services;

namespace ShopSana.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomeController(
        ILogger<HomeController> logger,
        IProductService productService,
        ICategoryService categoryService)
    {
        _logger = logger;
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            FeaturedProducts = await _productService.GetFeaturedProductsAsync(8),
            NewArrivals = await _productService.GetNewArrivalsAsync(8),
            Categories = await _categoryService.GetActiveCategoriesAsync()
        };

        return View(model);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
