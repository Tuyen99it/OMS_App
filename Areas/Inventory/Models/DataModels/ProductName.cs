using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMS_App.Areas.Inventory.Models
{

    public class ProductName
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public double Price { get; set; }
        public int Quantity { get; set; }
        public ICollection<ProductInventory> ProductInventories { get; set; }
        public ICollection<InventoryImage> ProductImages { get; set; }

        public ICollection<CategoryProduct> CategoriesProduct { get; set; }

    }
}