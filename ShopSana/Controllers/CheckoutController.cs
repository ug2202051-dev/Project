using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopSana.Models.Entities;
using ShopSana.Models.ViewModels;
using ShopSana.Services;
using System.Security.Claims;

namespace ShopSana.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(
            ICartService cartService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout") });
            }

            var cart = await _cartService.GetCartAsync(userId);
            if (!cart.CartItems.Any())
            {
                TempData["Warning"] = "Your cart is empty. Add some products before checkout.";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);

            var model = new CheckoutViewModel
            {
                Cart = cart,
                ShippingName = user?.FullName ?? string.Empty,
                ShippingAddress = user?.Address ?? string.Empty,
                ShippingCity = user?.City ?? string.Empty,
                ShippingPostalCode = user?.PostalCode ?? string.Empty,
                ShippingCountry = user?.Country ?? string.Empty,
                ShippingPhone = user?.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetCartAsync(userId);
            model.Cart = cart;

            if (!cart.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(userId, model);
                TempData["Success"] = "Order placed successfully!";
                return RedirectToAction("Confirmation", new { orderNumber = order.OrderNumber });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Index", model);
            }
        }

        public async Task<IActionResult> Confirmation(string orderNumber)
        {
            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != userId)
            {
                return Forbid();
            }

            return View(order);
        }
    }
}
