using Microsoft.Identity.Client;
using OMS_App.Areas.Orders.Dtos;

namespace OMS_App.Areas.Orders.Models
{
    public class OrderIndexViewModel
    {
        public int? OrderId { get; set; }
        public List<Order> OrdersReadDto { get; set; }

    }
}