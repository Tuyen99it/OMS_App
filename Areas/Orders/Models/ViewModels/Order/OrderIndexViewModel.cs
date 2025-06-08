using Microsoft.Identity.Client;
using OMS_App.Areas.Orders.Dtos;

namespace OMS_App.Areas.Orders.Models
{
    public class OrderIndexViewModel
    {
        public string SearchName { get; set; }
        public List<OrderReadDto> OrdersReadDto { get; set; }

    }
}