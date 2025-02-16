using System.ComponentModel.DataAnnotations;

namespace OMS_Webapp.Areas.Identity.Models
{ 
    public class LoginWith2faViewModel {
        [Required]
        [StringLength(7,ErrorMessage ="The {0} must be at lease {2} and at max {1} characters long",MinimumLength =6)]
        [DataType(DataType.Text)]
        [Display(Name ="Authentication code")]
        public string TwoFactorCode { get; set; }

        [Display(Name ="Remember this machine")]
        public bool RememberMachine {  get; set; }
    
    
    }




}
