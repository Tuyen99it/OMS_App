using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMS_App.Areas.Inventory.Models
{
    public class CategoryProduct
    {
        [Key]
        public int ProductNameId { get; set; }
        [ForeignKey("ProductNameId")]
        public ProductName ProductName { get; set; }
        public int ProductCategoryId { get; set; }
        [ForeignKey("ProductCategoryId")]
        public ProductCategory ProductCategory { get; set; }

    }
}