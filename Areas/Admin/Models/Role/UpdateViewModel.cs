using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace OMS_App.Areas.Admin.Role.Models{
    public class UpdateViewModel:IdentityRole{
       public string ClaimsString {get;set;}
       public string ClaimType {get;set;}
       public string ClaimValue{get;set;}
       

    }
}