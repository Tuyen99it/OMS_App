using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Models
{
    public class OrderedProduct
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int TotalProduct { get; set; }
        public double Price { get; set; }
        public double TotalPrices { get; set; }
        public bool IsOrder { get; set; }
        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]

        public AppUser User { get; set; }


    }
}