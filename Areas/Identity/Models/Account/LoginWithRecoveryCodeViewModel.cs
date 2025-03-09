using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OMS_App.Areas.Identity.Models
{
    public class LoginWithRecoveryCodeViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name ="Recovery Code")]
        public string RecoveryCode { get; set; }

    }
}
