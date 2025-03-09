using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OMS_App.Models;

namespace OMS_App.Areas.Admin.UserRole.Models{
    public class AddRoleViewModel{
        public AppUser User {get;set;}
        public string ExistingRoles {get;set;}
        public List<IdentityRole> TotalRoles {get;set;}

        public List<IdentityRole> RestOfRoles {get;set;}

    }
   
}