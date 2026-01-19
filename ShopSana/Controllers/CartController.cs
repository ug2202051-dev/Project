using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSana.Models.ViewModels;
using ShopSana.Services;
using System.Security.Claims;

namespace ShopSana.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetCartAsync(userId);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please login to add items to cart" });
            }

            try
            {
                await _cartService.AddToCartAsync(userId, productId, quantity);
                var count = await _cartService.GetCartItemCountAsync(userId);

                TempData["Success"] = "Item added to cart successfully!";
                return Json(new { success = true, cartCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int cartItemId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please login" });
            }

            try
            {
                await _cartService.UpdateCartItemAsync(userId, cartItemId, quantity);
                var cart = await _cartService.GetCartAsync(userId);

                return Json(new
                {
                    success = true,
                    subTotal = cart.SubTotal,
                    shipping = cart.ShippingCost,
                    tax = cart.Tax,
                    total = cart.Total,
                    cartCount = cart.ItemCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please login" });
            }

            var result = await _cartService.RemoveFromCartAsync(userId, cartItemId);
            if (result)
            {
                var cart = await _cartService.GetCartAsync(userId);
                return Json(new
                {
                    success = true,
                    subTotal = cart.SubTotal,
                    shipping = cart.ShippingCost,
                    tax = cart.Tax,
                    total = cart.Total,
                    cartCount = cart.ItemCount
                });
            }

            return Json(new { success = false, message = "Item not found" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            await _cartService.ClearCartAsync(userId);
            TempData["Success"] = "Cart cleared successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { count = 0 });
            }

            var count = await _cartService.GetCartItemCountAsync(userId);
            return Json(new { count });
        }
    }
}
