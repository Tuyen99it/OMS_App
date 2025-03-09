using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;
namespace OMS_App.Models
{
    public class AppRole
    {
        public  IdentityRole Role{get;set;}
        public AppRole(IdentityRole role){
            Role = role;
            ClaimsString = string.Empty;
        }
        
       public string ClaimsString {get;set;}

    }
}