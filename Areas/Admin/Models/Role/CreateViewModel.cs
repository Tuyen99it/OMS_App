using System.ComponentModel.DataAnnotations;

namespace OMS_App.Areas.Admin.Role.Models{
    public class CreateViewModel{
       [Required(ErrorMessage ="Role Name")]
        public string RoleName {get;set;}

        public string NormalName {get;set;}
        public string ClaimType {get;set;}
        public string ClaimValue {get;set;}

    }
}