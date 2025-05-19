using System.ComponentModel.DataAnnotations;

namespace OMS_App.Areas.Inventory.Models
{
    public class InventoryImage
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public ProductInventory ProductInventory { get; set; }
        public DateTime CreateDate { get; set; }
        public string ImageUrlPath { get; set; }

    }
}