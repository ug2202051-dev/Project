using ShopSana.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace ShopSana.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public CartViewModel Cart { get; set; } = new CartViewModel();

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(200)]
        [Display(Name = "Full Name")]
        public string ShippingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500)]
        [Display(Name = "Address")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        [Display(Name = "City")]
        public string ShippingCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(50)]
        [Display(Name = "Postal Code")]
        public string ShippingPostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100)]
        [Display(Name = "Country")]
        public string ShippingCountry { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? ShippingPhone { get; set; }

        [StringLength(500)]
        [Display(Name = "Order Notes")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class OrderConfirmationViewModel
    {
        public Order Order { get; set; } = null!;
        public IEnumerable<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderListViewModel
    {
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }

    public class OrderDetailsViewModel
    {
        public Order Order { get; set; } = null!;
        public IEnumerable<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
