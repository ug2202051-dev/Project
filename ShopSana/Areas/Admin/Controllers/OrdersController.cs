using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSana.Models.Entities;
using ShopSana.Models.ViewModels;
using ShopSana.Services;

namespace ShopSana.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(OrderStatus? status, string? search, int page = 1)
        {
            var orders = await _orderService.GetAllOrdersAsync(status, search, page, 20);
            var totalOrders = await _orderService.GetTotalOrderCountAsync(status);

            var model = new AdminOrderListViewModel
            {
                Orders = orders,
                StatusFilter = status,
                SearchTerm = search,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalOrders / 20.0)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _orderService.UpdateOrderStatusAsync(id, status);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = $"Order status updated to {status}.";
            return RedirectToAction("Details", new { id });
        }
    }
}
