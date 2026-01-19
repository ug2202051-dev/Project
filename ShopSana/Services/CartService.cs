using Microsoft.EntityFrameworkCore;
using ShopSana.Data;
using ShopSana.Models.Entities;
using ShopSana.Models.ViewModels;

namespace ShopSana.Services
{
    public interface ICartService
    {
        Task<CartViewModel> GetCartAsync(string userId);
        Task<CartItem> AddToCartAsync(string userId, int productId, int quantity);
        Task<CartItem?> UpdateCartItemAsync(string userId, int cartItemId, int quantity);
        Task<bool> RemoveFromCartAsync(string userId, int cartItemId);
        Task<bool> ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
    }

    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private const decimal TaxRate = 0.10m; // 10% tax
        private const decimal ShippingThreshold = 50.00m;
        private const decimal ShippingCost = 5.99m;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartViewModel> GetCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var subTotal = cartItems.Sum(c => c.Product.CurrentPrice * c.Quantity);
            var shipping = subTotal >= ShippingThreshold ? 0 : ShippingCost;
            var tax = subTotal * TaxRate;
            var total = subTotal + shipping + tax;

            return new CartViewModel
            {
                CartItems = cartItems,
                SubTotal = subTotal,
                ShippingCost = shipping,
                Tax = tax,
                Total = total,
                ItemCount = cartItems.Sum(c => c.Quantity)
            };
        }

        public async Task<CartItem> AddToCartAsync(string userId, int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || !product.IsActive)
                throw new ArgumentException("Product not found or unavailable");

            if (product.StockQuantity < quantity)
                throw new InvalidOperationException("Insufficient stock");

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + quantity;
                if (product.StockQuantity < newQuantity)
                    throw new InvalidOperationException("Insufficient stock");

                existingItem.Quantity = newQuantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return existingItem;
            }

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItem?> UpdateCartItemAsync(string userId, int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem == null)
                return null;

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return null;
            }

            if (cartItem.Product.StockQuantity < quantity)
                throw new InvalidOperationException("Insufficient stock");

            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> RemoveFromCartAsync(string userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return false;

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);
        }
    }
}
