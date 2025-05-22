using OMS_App.Areas.Inventory.Models;

namespace OMS_App.Areas.Inventory.Dto
{
    public class InventoryProductCreateDto
    {
         public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        
    }
}