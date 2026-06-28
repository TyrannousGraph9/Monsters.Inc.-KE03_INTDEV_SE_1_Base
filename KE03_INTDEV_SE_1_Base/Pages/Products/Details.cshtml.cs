using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_1_Base.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages.Products
{
    public class ProductDetailsModel : PageModel
    {
        private const string CartSessionKey = "ShoppingCart";

        private readonly IProductRepository _productRepository;

        public Product Product { get; private set; } = null!;

        public ProductDetailsModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult OnGet(int id)
        {
            var product = _productRepository.GetProductById(id);

            if (product is null)
            {
                return NotFound();
            }

            Product = product;
            return Page();
        }

        public IActionResult OnPostAddToCart(int productId)
        {
            var product = _productRepository.GetProductById(productId);

            if (product is null)
            {
                return RedirectToPage(new { id = productId });
            }

            var cart = HttpContext.Session.GetObject<Dictionary<int, int>>(CartSessionKey) ?? new Dictionary<int, int>();

            if (cart.ContainsKey(productId))
            {
                cart[productId]++;
            }
            else
            {
                cart[productId] = 1;
            }

            HttpContext.Session.SetObject(CartSessionKey, cart);

            return RedirectToPage("/shoppingcart");
        }
    }
}
