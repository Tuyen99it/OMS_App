using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace OMS_Webapp.Models
{
    public class AppUser : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; }
        [MaxLength(255)]
        public string Address { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

    }
}