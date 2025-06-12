using Microsoft.Identity.Client;
using OMS_App.Areas.Orders.Dtos;

namespace OMS_App.Areas.Orders.Models
{
    public class UpdateOrderViewModel
    {
        public OrderStatusUpdate OrderStatusUpdate { get; set; }
        public List<OrderStatus> ExistStatus { get; set; }

    }
}