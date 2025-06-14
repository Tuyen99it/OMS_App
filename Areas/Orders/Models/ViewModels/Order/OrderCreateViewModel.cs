using Microsoft.Identity.Client;
using OMS_App.Areas.Orders.Dtos;

namespace OMS_App.Areas.Orders.Models
{
    public class OrderCreateViewModel
    {
        public OrderCreateDto OrderCreateDto { get; set; }
        public List<OrderReadDto> OrdersReadDto { get; set; }

    }
}