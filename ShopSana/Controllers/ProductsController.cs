using Microsoft.AspNetCore.Mvc;
using ShopSana.Models.ViewModels;
using ShopSana.Services;

namespace ShopSana.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId, string? search, string? sort, int page = 1)
        {
            var model = await _productService.GetProductsAsync(categoryId, search, sort, page, 12);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductDetailsViewModel
            {
                Product = product,
                RelatedProducts = await _productService.GetRelatedProductsAsync(id, 4)
            };

            return View(model);
        }

        public async Task<IActionResult> Category(int id)
        {
            return RedirectToAction("Index", new { categoryId = id });
        }

        public async Task<IActionResult> Search(string q)
        {
            return RedirectToAction("Index", new { search = q });
        }
    }
}
