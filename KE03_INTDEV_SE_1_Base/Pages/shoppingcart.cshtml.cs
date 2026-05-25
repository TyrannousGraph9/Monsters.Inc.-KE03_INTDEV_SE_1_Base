using DataAccessLayer.Interfaces;
using KE03_INTDEV_SE_1_Base.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
	public class ShoppingcartModel : PageModel
	{
		private const string CartSessionKey = "ShoppingCart";

		private readonly IProductRepository _productRepository;

		public IList<CartItem> CartItems { get; private set; } = new List<CartItem>();

		public decimal Subtotal => CartItems.Sum(item => item.LineTotal);

		public decimal Shipping => CartItems.Count == 0 ? 0 : 0m;

		public decimal Total => Subtotal + Shipping;

		public ShoppingcartModel(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public void OnGet()
		{
			LoadCartItems();
		}

		public IActionResult OnPostIncrease(int productId)
		{
			UpdateQuantity(productId, 1);
			return RedirectToPage();
		}

		public IActionResult OnPostDecrease(int productId)
		{
			UpdateQuantity(productId, -1);
			return RedirectToPage();
		}

		public IActionResult OnPostRemove(int productId)
		{
			var cart = GetCart();

			if (cart.Remove(productId))
			{
				SaveCart(cart);
			}

			return RedirectToPage();
		}

		private void LoadCartItems()
		{
			var cart = GetCart();
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

		private Dictionary<int, int> GetCart()
		{
			return HttpContext.Session.GetObject<Dictionary<int, int>>(CartSessionKey) ?? new Dictionary<int, int>();
		}

		private void SaveCart(Dictionary<int, int> cart)
		{
			HttpContext.Session.SetObject(CartSessionKey, cart);
		}

		private void UpdateQuantity(int productId, int delta)
		{
			var cart = GetCart();

			if (!cart.TryGetValue(productId, out var currentQuantity))
			{
				return;
			}

			var updatedQuantity = currentQuantity + delta;

			if (updatedQuantity <= 0)
			{
				cart.Remove(productId);
			}
			else
			{
				cart[productId] = updatedQuantity;
			}

			SaveCart(cart);
		}

		public sealed record CartItem(int ProductId, string Name, string Description, int Quantity, decimal UnitPrice, string? ImageUrl = null)
		{
			public decimal LineTotal => Quantity * UnitPrice;
		}
	}
}
