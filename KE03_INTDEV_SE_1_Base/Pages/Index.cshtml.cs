using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_1_Base.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class IndexModel : PageModel
    {
        private const string CartSessionKey = "ShoppingCart";

        private readonly ILogger<IndexModel> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public IList<Category> Categories { get; set; }
        public IList<Product> Products { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterCategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "name_asc";

        public IndexModel(
            ILogger<IndexModel> logger,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            Categories = new List<Category>();
            Products = new List<Product>();
        }

        public void OnGet()
        {
            Categories = _categoryRepository.GetAllCategories().ToList();

            var productsQuery = _productRepository.GetAllProducts().AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                productsQuery = productsQuery.Where(p =>
                    (p.Name != null && p.Name.Contains(SearchString, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Description != null && p.Description.Contains(SearchString, StringComparison.OrdinalIgnoreCase)));
            }

            if (FilterCategoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == FilterCategoryId);
            }

            Products = SortOrder switch
            {
                "name_desc" => productsQuery.OrderByDescending(p => p.Name).ToList(),
                "price_asc" => productsQuery.OrderBy(p => p.Price).ToList(),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price).ToList(),
                _ => productsQuery.OrderBy(p => p.Name).ToList(),
            };

            _logger.LogInformation("Loaded {ProductCount} products after filtering and sorting.", Products.Count);
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
