using Microsoft.EntityFrameworkCore;
using ShopSana.Data;
using ShopSana.Models.Entities;
using ShopSana.Models.ViewModels;

namespace ShopSana.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, CheckoutViewModel model);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order?> GetOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId, int page = 1, int pageSize = 10);
        Task<int> GetUserOrderCountAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync(OrderStatus? status = null, string? searchTerm = null, int page = 1, int pageSize = 20);
        Task<int> GetTotalOrderCountAsync(OrderStatus? status = null);
        Task<Order?> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetTodayRevenueAsync();
        Task<int> GetPendingOrderCountAsync();
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private const decimal TaxRate = 0.10m;
        private const decimal ShippingThreshold = 50.00m;
        private const decimal ShippingCost = 5.99m;

        public OrderService(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<Order> CreateOrderAsync(string userId, CheckoutViewModel model)
        {
            var cart = await _cartService.GetCartAsync(userId);
            if (!cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty");

            // Verify stock and calculate totals
            foreach (var item in cart.CartItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || !product.IsActive)
                    throw new InvalidOperationException($"Product '{item.Product.Name}' is no longer available");
                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for '{item.Product.Name}'");
            }

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = userId,
                SubTotal = cart.SubTotal,
                ShippingCost = cart.ShippingCost,
                Tax = cart.Tax,
                TotalAmount = cart.Total,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                PaymentMethod = model.PaymentMethod,
                ShippingName = model.ShippingName,
                ShippingAddress = model.ShippingAddress,
                ShippingCity = model.ShippingCity,
                ShippingPostalCode = model.ShippingPostalCode,
                ShippingCountry = model.ShippingCountry,
                ShippingPhone = model.ShippingPhone,
                Notes = model.Notes
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order items and update stock
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    UnitPrice = cartItem.Product.CurrentPrice,
                    Quantity = cartItem.Quantity,
                    Discount = cartItem.Product.HasDiscount ? cartItem.Product.Price - cartItem.Product.CurrentPrice : 0,
                    TotalPrice = cartItem.Product.CurrentPrice * cartItem.Quantity
                };

                _context.OrderItems.Add(orderItem);

                // Update product stock
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= cartItem.Quantity;
                }
            }

            await _context.SaveChangesAsync();

            // Clear the cart
            await _cartService.ClearCartAsync(userId);

            // Mark payment as paid for demo (in real app, integrate payment gateway)
            order.PaymentStatus = PaymentStatus.Paid;
            order.TransactionId = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{order.Id}";
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId, int page = 1, int pageSize = 10)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUserOrderCountAsync(string userId)
        {
            return await _context.Orders.CountAsync(o => o.UserId == userId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(OrderStatus? status = null, string? searchTerm = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(o => o.OrderNumber.ToLower().Contains(term) ||
                                        o.User.Email!.ToLower().Contains(term) ||
                                        o.ShippingName.ToLower().Contains(term));
            }

            return await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalOrderCountAsync(OrderStatus? status = null)
        {
            var query = _context.Orders.AsQueryable();
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }
            return await query.CountAsync();
        }

        public async Task<Order?> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return null;

            order.Status = status;

            if (status == OrderStatus.Shipped)
                order.ShippedDate = DateTime.UtcNow;
            else if (status == OrderStatus.Delivered)
                order.DeliveredDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Orders
                .Where(o => o.PaymentStatus == PaymentStatus.Paid && o.OrderDate.Date == today)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetPendingOrderCountAsync()
        {
            return await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }
    }
}
