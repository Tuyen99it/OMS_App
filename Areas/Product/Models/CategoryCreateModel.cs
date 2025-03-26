using Microsoft.AspNetCore.Mvc.Rendering;
using OMS_App.Models;
namespace OMS_App.Areas.Product.Models
{
    public class CategoryCreateModel
    {
        public Category Category { get; set; }  
        public int? ParentCategoryId { get; set; }
        public List<SelectListItem> Options { get;set; }
       
    }
}
