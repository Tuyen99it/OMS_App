using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMS_App.Areas.Inventory.Models
{
    public class CategoryProduct
    {
        [Key]
        public int ProductInventoryId { get; set; }
        [ForeignKey("ProductInventoryId")]
        public ProductInventory ProductInventory { get; set; }
        public int ProductCategoryId { get; set; }
        [ForeignKey("ProductCategoryId")]
        public ProductCategory ProductCategory { get; set; }

    }
}