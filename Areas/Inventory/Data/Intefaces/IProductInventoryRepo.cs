using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Areas.Inventory.Data
{
    public interface IProductInventoryRepo
    {
        Task<List<ProductInventory>> GetAllProductInventoryAsync(int itemShowNumber, int existPage);
        Task<ProductInventory> GetProductInventoryByIdAsync(string productId);
        Task<int> GetQuantityByIdAsync(int productId);
        Task<bool> CreateProductInventoryAsync(ProductInventory product);
        Task<bool> UpdateProductInventoryAsync(ProductInventory product);
        Task<bool> DeleteProductInventoryAsync(ProductInventory product);

    }
}