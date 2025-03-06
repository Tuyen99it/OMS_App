using System.ComponentModel.DataAnnotations;

namespace OMS_App.Areas.Admin.Models{
    public class RoleManagement{
        [Display(Name ="Search")]
        public string? SearchingRoleName{get;set;}

    }
}