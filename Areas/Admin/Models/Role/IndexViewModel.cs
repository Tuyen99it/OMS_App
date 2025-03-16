using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OMS_App.Areas.Admin.Role.Models{
    public class IndexViewModel{

        [Display(Name ="Search")]
        public string? SearchingRoleName{get;set;}
        
    }
   
}