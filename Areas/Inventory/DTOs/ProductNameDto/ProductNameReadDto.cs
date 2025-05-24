using OMS_App.Areas.Inventory.Models;

namespace OMS_App.Areas.Inventory.Dtos
{
    public class ProductNameReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ProductInventory> ProductInventories { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public ICollection<InventoryImage> ProductImages { get; set; }

        public ICollection<CategoryProduct> CategoriesProduct { get; set; }

    }
}