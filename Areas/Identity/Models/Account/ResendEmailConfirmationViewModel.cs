using System.ComponentModel.DataAnnotations;

namespace OMS_App.Areas.Identity.Models
{
    public class ResendEmailConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
