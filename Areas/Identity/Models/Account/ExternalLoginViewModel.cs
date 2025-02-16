using System.ComponentModel.DataAnnotations;

namespace OMS_Webapp.Areas.Identity.Models
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
