using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly ICategoryRepository _categoryRepository;

        public IList<Category> Categories { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            Categories = new List<Category>();
        }

        public void OnGet()
        {            
            Categories = _categoryRepository.GetAllCategories().ToList();
            _logger.LogInformation($"getting all {Categories.Count} categories from the database");
        }
    }
}
