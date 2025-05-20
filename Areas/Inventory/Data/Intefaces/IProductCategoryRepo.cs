using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Areas.Inventory.Data
{
    public interface IProductCategoryRepo
    {
        Task<List<ProductCategory>> GetAllProductCategoriesAsync();
        Task<ProductCategory> GetProductCategoryByIdAsync(string categoryId);
        Task<bool> CreateAsync(ProductCategory category);
        Task<bool> UpdateAsync(ProductCategory category);
        Task<bool> DeleteAsync(ProductCategory category);

    }
}