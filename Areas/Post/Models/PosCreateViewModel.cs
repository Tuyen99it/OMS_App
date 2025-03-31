using Microsoft.AspNetCore.Mvc.Rendering;
using OMS_App.Models;
namespace OMS_App.Areas.Post.Models
{
    public class PostCreateViewModel
    {
        public Post Post { get; set; }  
        public int[] CategoriesId { get; set; }
        public MultiSelectList Options { get;set; }
        public AppUser Author {get;set;}
       
    }
}
