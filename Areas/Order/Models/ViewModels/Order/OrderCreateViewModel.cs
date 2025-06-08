using Microsoft.Identity.Client;
using OMS_App.Areas.Inventory.Dtos;
using OMS_App.Areas.Orders.Dtos;
using OMS_App.Areas.Orders.Models;

namespace OMS_App.Areas.Inventory.Models
{
    public class OrderCreateViewModel
    {
       public OrderCreateDto OrderCreateDto {get;set;}
        public List<OrderReadDto> OrdersReadDto { get; set; }


    }
}