using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_1_Base.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class AssortimentModel : PageModel
    {
        private const string CartSessionKey = "ShoppingCart";

        private readonly ILogger<AssortimentModel> _logger;

        private readonly IProductRepository _productRepository;

        public IList<Product> Products { get; set; }

        public AssortimentModel(ILogger<AssortimentModel> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            Products = new List<Product>();
        }

        public void OnGet()
        {            
            Products = _productRepository.GetAllProducts().ToList();
            _logger.LogInformation($"getting all {Products.Count} products from the database");
        }

        public IActionResult OnPostAddToCart(int productId)
        {
            var product = _productRepository.GetProductById(productId);

            if (product is null)
            {
                return RedirectToPage();
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
