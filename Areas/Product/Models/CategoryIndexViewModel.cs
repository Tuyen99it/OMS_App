using Microsoft.AspNetCore.Mvc.Rendering;
using OMS_App.Models;
namespace OMS_App.Areas.Product.Models
{
    public class CategoryIndexViewModel
    {
        public List<Category>Categories { get; set; }  
        public List<SelectListItem>Options { get; set; }

       
    }
}
