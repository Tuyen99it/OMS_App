using OMS_App.Areas.Inventory.Models;

namespace OMS_App.Areas.Inventory.Dtos
{
    public class InventoryImageUpdateDto
    {
        public int Id { get; set; }
        public ImageType Type { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductInventoryId { get; set; }
    }
}