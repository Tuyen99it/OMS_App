using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int OrderedProductId { get; set; }
        [ForeignKey("OrderedProductId")]
        public ICollection<OrderedProduct> OrderedProducts { get; set; }
        public Dictionary<OrderStatus, DateTime> UpdateStatus { get; set; }
        public List<Dictionary<OrderStatus, DateTime>> UpdateStatuses { get; set; }
        public int OrderStatus { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        public double OrderedPriceTotal { get; set; }
        public OrderAddress Address { get; set; }

    }
    public enum OrderStatus
    {
        Create,
        Send,
        Delivery,
        Paid,
        Finish
    }
}