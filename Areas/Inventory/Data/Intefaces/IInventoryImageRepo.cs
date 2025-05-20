using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Areas.Inventory.Data
{
    public interface IInventoryImage
    {
        Task<List<InventoryImage>> GetAllImages(string imageTypeId);
        Task<bool> CreateCategoryImageAsync(string typeImageId, InventoryImage image);
        Task<bool> CreateProductImageAsync(string typeImageId, InventoryImage image);
        Task<bool> UpdateCategoryImageAsync(string typeImageId, InventoryImage image);
        Task<bool> UpdateProductImageAsync(string typeImageId, InventoryImage image);
         Task<bool> DeleteCategoryImageAsync(string typeImageId, InventoryImage image);
        Task<bool> DeleteProductImageAsync(string typeImageId, InventoryImage image);
       

    }
}