using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMS_App.Areas.Inventory.Models
{

    public class ProductInventory
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime ExpireDate { get; set; }


        public int ProductNameId { get; set; }
        [ForeignKey("ProductNameId")]
        public ProductName ProductName { get; set; }
    }
}