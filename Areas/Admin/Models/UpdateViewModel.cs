using System.ComponentModel.DataAnnotations;

namespace OMS_App.Areas.Admin.Models{
    public class UpdateViewModel{
       [Required(ErrorMessage ="Role Name")]
        public string RoleName {get;set;}

        public string NormalName {get;set;}

    }
}