using System.ComponentModel.DataAnnotations;
namespace OMS_App.Areas.Identity.Models
{
    public class DeletePersonalDataViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
    }
}