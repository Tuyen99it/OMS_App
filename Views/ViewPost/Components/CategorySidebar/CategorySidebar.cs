using Microsoft.AspNetCore.Mvc;
using OMS_App.Models;
[ViewComponent]
public class CategorySidebar: ViewComponent{
    public class CategorySidebarData{
        public List<Category>categories {get;set;}
        public int Level {get;set;}
        public string slugCategory {get;set;}
    }
    public const string COMPONENT_NAME="CategorySidebar";
    public CategorySidebar(){}
    public IViewComponentResult Invoke(CategorySidebarData data){

        return View(data);
    }

}