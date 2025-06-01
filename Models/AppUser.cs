using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using OMS_App.Areas.Orders.Models;
namespace OMS_App.Models
{
    public class AppUser : IdentityUser
    {
        [MaxLength(100)]
        public string? FullName { get; set; }
        [MaxLength(255)]
        public string? Address { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        public ICollection<UserImage> UserImages { get; set; }
        public ICollection<OrderAddress> OrderAddressed { get; set; }
        public ICollection<Order> Orders { get; set; }

    }
}