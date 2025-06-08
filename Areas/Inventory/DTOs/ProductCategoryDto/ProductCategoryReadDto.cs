using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Inventory.Models;

namespace OMS_App.Areas.Inventory.Dtos
{
    public class ProductCategoryReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
     
        public ProductCategory ParentCategory { get; set; }

        public ICollection<ProductCategory> ChildrenCategory { get; set; }
        public ICollection<InventoryImage> CategoryImages { get; set; }
    }
}