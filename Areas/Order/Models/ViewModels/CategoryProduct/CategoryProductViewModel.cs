using Microsoft.Identity.Client;
using OMS_App.Areas.Inventory.Dtos;

namespace OMS_App.Areas.Inventory.Models
{
    public class CategoryProductViewModel
    {
        public ProductCategoryCreateDto CategoryProductCreateDto { get; set; }
        public List<ProductCategoryReadDto> CategoriesProductReadDto { get; set; }

        public int CategoryId { get; set; }
    }
}