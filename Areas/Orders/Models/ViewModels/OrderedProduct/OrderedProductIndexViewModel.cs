using Microsoft.Identity.Client;
using OMS_App.Areas.Orders.Dtos;

namespace OMS_App.Areas.Orders.Models
{
    public class OrderedProductIndexViewModel
    {
        public string SearchName { get; set; }
        public List<OrderedProductReadDto> OrderedProductsReadDto { get; set; }

    }
}