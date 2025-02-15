using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace OMS_Webapp.Areas.Identity.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Please enter {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Code { get;set; }

    }



}
