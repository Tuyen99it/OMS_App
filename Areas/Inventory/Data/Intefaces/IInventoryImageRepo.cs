using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Areas.Inventory.Data
{
    public interface IInventoryImageRepo
    {
        Task<List<InventoryImage>> GetAllImages(string imageTypeId,ImageType type);
        Task<bool> CreateImageAsync( InventoryImage image);
        Task<bool> UpdateImageAsync(InventoryImage image);  
         Task<bool> DeleteImageAsync(InventoryImage image);

    }
}