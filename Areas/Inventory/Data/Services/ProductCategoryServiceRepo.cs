using AspNetCoreGeneratedDocument;
using Microsoft.EntityFrameworkCore;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Data;

namespace OMS_App.Areas.Inventory.Data
{
    public class ProductCategoryServiceRepo : IProductCategoryRepo
    {
        private readonly OMSDBContext _context;
        private readonly ILogger<ProductCategoryServiceRepo> _logger;

        public ProductCategoryServiceRepo(OMSDBContext context, ILogger<ProductCategoryServiceRepo> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<bool> CreateAsync(ProductCategory category)
        {
            if (category == null)
            {
                _logger.LogError("Category is null");
                return false;
            }
            var existCategory = _context.ProductCategories.Any(c => c.Id == category.Id);
            if (existCategory)
            {
                _logger.LogWarning("Category is existing");
                return false;
            }
            await _context.ProductCategories.AddAsync(category);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> DeleteAsync(ProductCategory category)
        {
            if (category == null)
            {
                _logger.LogError("Category is null");
                return false;
            }
            var existCategory = await _context.ProductCategories.AnyAsync(c => c.Id == category.Id);
            if (!existCategory)
            {
                _logger.LogWarning("Category is not existing");
                return false;
            }
            _context.ProductCategories.Remove(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<ProductCategory>> GetAllProductCategoriesAsync()
        {
            var categories = await _context.ProductCategories
                                           .Where(c => c.ParentCategoryId == null)
                                            .Include(c => c.ChildrenCategory)
                                            .Include(c => c.CategoryImages)
                                           .ToListAsync();
            return categories;
        }

        public async Task<ProductCategory> GetProductCategoryByIdAsync(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                _logger.LogError("Category is Empty");
                return null;
            }
            var category = _context.ProductCategories
                                   .Where(c => c.Id == Convert.ToInt16(categoryId))
                                   .Include(c => c.ChildrenCategory)
                                   .Include(c => c.CategoryImages)
                                   .FirstOrDefault();
            return category;
        }

        public async Task<bool> UpdateAsync(ProductCategory category)
        {
            if (category == null)
            {
                _logger.LogError("Category is null");
                return false;
            }
            var existCategory = _context.ProductCategories.Where(c => c.Id == category.Id).FirstOrDefault();
            if (existCategory == null)
            {
                _logger.LogError("There is no category in the database");
                return false;

            }
            _context.ProductCategories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}