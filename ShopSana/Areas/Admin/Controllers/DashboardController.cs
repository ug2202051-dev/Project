using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopSana.Models.Entities;
using ShopSana.Models.ViewModels;
using ShopSana.Services;

namespace ShopSana.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            IOrderService orderService,
            IProductService productService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _productService = productService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalOrders = await _orderService.GetTotalOrderCountAsync(),
                PendingOrders = await _orderService.GetPendingOrderCountAsync(),
                TotalProducts = (await _productService.GetProductsAsync(null, null, null, 1, 1)).TotalProducts,
                TotalUsers = _userManager.Users.Count(),
                TotalRevenue = await _orderService.GetTotalRevenueAsync(),
                TodayRevenue = await _orderService.GetTodayRevenueAsync(),
                RecentOrders = await _orderService.GetAllOrdersAsync(null, null, 1, 5),
                LowStockProducts = await _productService.GetLowStockProductsAsync(10)
            };

            return View(model);
        }
    }
}
