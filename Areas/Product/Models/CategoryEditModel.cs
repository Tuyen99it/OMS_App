using Microsoft.AspNetCore.Mvc.Rendering;
using OMS_App.Models;
namespace OMS_App.Areas.Product.Models
{
    public class CategoryEditModel
    {
        public Category Category { get; set; }  
        
        public List<SelectListItem> Options { get;set; }
       
    }
}
