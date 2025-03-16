using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OMS_App.Models;

namespace OMS_App.Areas.Admin.UserRole.Models
{
    public class AddRoleViewModel
    {
        public AppUser User { get; set; }
        public string ExistingRole { get; set; }
        public string SelectedRole { get; set; }
        public List<string> RestOfRoles { get; set; }
        public List<string> ExistingRoles { get; set; }
        public List<SelectListItem> Options { get; set; }



    }

}