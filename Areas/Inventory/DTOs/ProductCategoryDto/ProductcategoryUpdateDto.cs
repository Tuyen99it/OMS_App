using OMS_App.Areas.Inventory.Models;

namespace OMS_App.Areas.Inventory.Dto
{
    public class ProductCategoryUpdateDto
    {
         public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}