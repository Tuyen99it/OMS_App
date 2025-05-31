using Microsoft.Identity.Client;
using OMS_App.Areas.Inventory.Dtos;

namespace OMS_App.Areas.Inventory.Models
{
    public class ProductNameIndexViewModel
    {
        public string SearchName { get; set; }
        public List<ProductNameReadDto> ProductsNameReadDto { get; set; }

    }
}