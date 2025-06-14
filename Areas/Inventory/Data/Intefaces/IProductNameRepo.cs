using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Areas.Inventory.Data
{
    public interface IProductNameRepo
    {
        Task<List<ProductName>> GetAllProductNameAsync(int itemShowNumber, int existPage);
        Task<List<ProductName>> GetNumbersProductAsync(int numberProduct);
        Task<List<ProductName>> GetProductNameByNameAsync(string searchname, int itemShowNumber, int existPage);
        Task<ProductName> GetProductNameByIdAsync(string productId);
        Task<List<ProductName>> GetLastProductsByNumberAsync(int number);
        Task<ProductName> GetProductNameByNameAsync(string productName);
        Task<bool> CreateProductNameAsync(ProductName product);
        Task<bool> UpdateProductNameAsync(ProductName product);
        Task<bool> DeleteProductNameAsync(ProductName product);

    }
}