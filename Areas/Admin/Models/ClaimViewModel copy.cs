using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OMS_App.Areas.Admin.Models
{
    public class ClaimCreateViewModel
    {
       
        [Required]
        public string ClaimType { get; set; }
        [Required]
        public string ClaimValue { get; set; }

    }

}