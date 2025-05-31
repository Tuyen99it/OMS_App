using Microsoft.Identity.Client;
using OMS_App.Areas.Inventory.Dtos;

namespace OMS_App.Areas.Inventory.Models
{
    public class CategoryProductIndexViewModel
    {
        public string SearchName { get; set; }
        public List<ProductCategoryReadDto> CategoriesProductReadDto { get; set; }

    }
}