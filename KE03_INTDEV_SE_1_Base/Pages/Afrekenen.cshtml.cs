using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_1_Base.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class AfrekenenModel : PageModel
    {
        private const string CartSessionKey = "ShoppingCart";

        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        [BindProperty]
        public CheckoutInput Input { get; set; } = new();

        public IList<CartItem> CartItems { get; private set; } = new List<CartItem>();

        public decimal Subtotal => CartItems.Sum(item => item.LineTotal);

        public decimal Shipping => CartItems.Count == 0 ? 0 : 0m;

        public decimal Total => Subtotal + Shipping;

        public AfrekenenModel(
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public void OnGet()
        {
            LoadCartItems();
        }

        public IActionResult OnPostPlaceOrder()
        {
            LoadCartItems();

            if (!CartItems.Any())
            {
                ModelState.AddModelError(string.Empty, "Je winkelmandje is leeg.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var customer = new Customer
            {
                Name = $"{Input.FirstName} {Input.LastName}".Trim(),
                Address = $"{Input.Street}, {Input.Postcode} {Input.City}".Trim(),
                Active = true
            };

            _customerRepository.AddCustomer(customer);

            var order = new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.Now
            };

            foreach (var cartItem in CartItems)
            {
                var product = _productRepository.GetProductById(cartItem.ProductId);

                if (product is null)
                {
                    continue;
                }

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice
                });
            }

            _orderRepository.AddOrder(order);
            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToPage("/Orders");
        }

        private void LoadCartItems()
        {
            var cart = HttpContext.Session.GetObject<Dictionary<int, int>>(CartSessionKey) ?? new Dictionary<int, int>();
            var cartItems = new List<CartItem>();

            foreach (var cartEntry in cart)
            {
                var product = _productRepository.GetProductById(cartEntry.Key);

                if (product is null)
                {
                    continue;
                }

                cartItems.Add(new CartItem(
                    product.Id,
                    product.Name,
                    product.Description,
                    cartEntry.Value,
                    product.Price,
                    product.Image is { Length: > 0 } imageBytes ? $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}" : null));
            }

            CartItems = cartItems;
        }

        public sealed record CartItem(int ProductId, string Name, string Description, int Quantity, decimal UnitPrice, string? ImageUrl = null)
        {
            public decimal LineTotal => Quantity * UnitPrice;
        }

        public sealed class CheckoutInput
        {
            [Required]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Street { get; set; } = string.Empty;

            [Required]
            public string Postcode { get; set; } = string.Empty;

            [Required]
            public string City { get; set; } = string.Empty;

            [Required]
            public string PaymentMethod { get; set; } = string.Empty;
        }
    }
}