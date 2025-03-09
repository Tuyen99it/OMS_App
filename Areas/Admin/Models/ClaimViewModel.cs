using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OMS_App.Areas.Admin.Models
{
    public class ClaimViewModel
    {
        public string? SearchingClaimName {get;set;}
        [Required]
        public string ClaimType { get; set; }
        [Required]
        public string ClaimValue { get; set; }

    }

}