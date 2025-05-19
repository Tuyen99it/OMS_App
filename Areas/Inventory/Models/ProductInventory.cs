using System.ComponentModel.DataAnnotations;

namespace OMS_App.Areas.Inventory.Models
{
    public class ProductInventory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public ICollection<CategoryProduct> CategoriesProduct { get; set; }

    }
}