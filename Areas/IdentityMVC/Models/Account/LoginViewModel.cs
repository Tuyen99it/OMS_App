using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace OMS_Webapp.Areas.Identity.Models
{
    public class LoginViewModel 
    {
        [Required(ErrorMessage ="Please enter {0}")]
        [Display(Name ="User or Email")]
        public string UserName { get;set; }
        [Required(ErrorMessage ="Please enter {0}")]
        [DataType(DataType.Password)]
        [Display(Name ="Password")]
        public string Password { get;set; }
        [Display(Name ="Remember login")]
        public bool RememberMe { get; set;}
      
    }



}
