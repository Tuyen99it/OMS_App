   using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public bool IsNewCreate { get; set; } = true;

        public ICollection<OrderedProduct> OrderedProducts { get; set; }

        public ICollection<OrderStatusUpdate> OrderStatusUpdates { get; set; }
        public string UserId { get; set; }

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
        Finish,
        Cancel
    }
    public class OrderStatusUpdate
    {
        [Key]
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public bool? IsStatusUpdate { get; set; } = false;

        public DateTime? UpdateTime { get; set; }
        public string Note { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }

}