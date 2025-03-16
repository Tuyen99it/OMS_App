using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using OMS_App.Models;

namespace OMS_App.Areas.Admin.UserRole.Models{
    public class UserListModel{

        [Display(Name ="Search")] 
        public string? SearchUserName {get;set;}
        public List<UserAndRole>? Users {get;set;}
        
    }
    public class UserAndRole:AppUser{
        public string RoleNames{get;set;}
    }
   
}