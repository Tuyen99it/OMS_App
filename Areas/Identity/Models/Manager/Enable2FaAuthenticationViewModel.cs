using System.ComponentModel.DataAnnotations;

namespace OMS_App.Areas.Identity.Models{
    public class Enable2FaAuthenticationViewModel{
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }
        [Required]
        [StringLength(7, ErrorMessage = "Mã xác thực phải có ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Mã xác thực")]
        public string Code { get; set; }
    }
}