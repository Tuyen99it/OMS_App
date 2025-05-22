using OMS_App.Areas.Inventory.Models;

namespace OMS_App.Areas.Inventory.Dto
{
    public class ProductCategoryReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }

        public ICollection<ProductCategory> ChildrenCategory { get; set; }
        public ICollection<InventoryImage> CategoryImages { get; set; }
    }
}