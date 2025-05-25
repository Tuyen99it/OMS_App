using Microsoft.Identity.Client;
using OMS_App.Areas.Inventory.Dtos;

namespace OMS_App.Areas.Inventory.Models
{
    public class ProductInventoryViewModel
    {
        public ProductNameCreateDto ProductNameCreateDto { get; set; }
        public List<ProductNameReadDto> ProductsNameReadDto { get; set; }
        public int number { get; set; }
    }
}