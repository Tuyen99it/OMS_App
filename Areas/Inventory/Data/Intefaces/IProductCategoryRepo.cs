using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Areas.Inventory.Data
{
    public interface IProductCategoryRepo
    {
        Task<List<ProductCategory>> GetAllProductCategoriesAsync();
        Task<List<ProductCategory>> GetCategoriesProductByNameAsync(string searchCategory);

        Task<ProductCategory> GetCategoryProductByIdAsync(string categoryId);
        Task<ProductCategory> GetCategoryProductByNameAsync(string categoryName);
        Task<bool> CreateAsync(ProductCategory category);
        Task<bool> UpdateAsync(ProductCategory category);
        Task<bool> DeleteAsync(ProductCategory category);

    }
}