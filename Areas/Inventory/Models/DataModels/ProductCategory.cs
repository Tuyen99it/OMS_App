
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NuGet.Protocol.Plugins;

namespace OMS_App.Areas.Inventory.Models
{

    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
        [ForeignKey(nameof(ParentCategoryId))]
        public ProductCategory productCategory { get; set; }
        public ICollection<ProductCategory> ChildrenCategory { get; set; }
        public ICollection<InventoryImage> CategoryImages { get; set; }
    }
}