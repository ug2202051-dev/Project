using ShopSana.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace ShopSana.Models.ViewModels
{
    public class CartViewModel
    {
        public IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
    }

    public class AddToCartViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemViewModel
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
