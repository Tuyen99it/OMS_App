using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Areas.Inventory.Data
{
    public interface IProductNameRepo
    {
        Task<List<ProductName>> GetAllProductNameAsync(int itemShowNumber, int existPage);
        Task<List<ProductName>> GetProductsInventoryByNameAsync(string searchname,int itemShowNumber, int existPage);
        Task<ProductName> GetProductNameByIdAsync(string productId);
        Task<ProductName> GetProductNameByNameAsync(string productName);
        Task<bool> CreateProductNameAsync(ProductName product);
        Task<bool> UpdateProductNameAsync(ProductName product);
        Task<bool> DeleteProductNameAsync(ProductName product);

    }
}