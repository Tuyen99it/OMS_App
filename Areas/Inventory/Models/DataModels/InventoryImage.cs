using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMS_App.Areas.Inventory.Models
{
 
    public class InventoryImage
    {
        [Key]
        public int Id { get; set; }
        public ImageType Type { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductNameId { get; set; }
        [ForeignKey("ProductCategoryId")]
        public ProductCategory ProductCategory { get; set; }
        [ForeignKey("ProductNameId")]
        public ProductName ProductName { get; set; }
        public DateTime CreateDate { get; set; }
        public string ImageUrlPath { get; set; }


    }
    public enum ImageType { ProductInventory, ProductCategory }
    
}