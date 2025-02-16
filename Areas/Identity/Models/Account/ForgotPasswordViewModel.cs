using System.ComponentModel.DataAnnotations;

namespace OMS_Webapp.Areas.Identity.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage ="Please enter email.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name ="Email Address")]
        public string Email { get;set; }
    }
}
