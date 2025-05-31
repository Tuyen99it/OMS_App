using OMS_App.Areas.Inventory.Models;

namespace OMS_App.Areas.Inventory.Dtos
{
    public class ProductCategoryCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }

    }
}