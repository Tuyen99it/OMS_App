using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OMS_App.Areas.Identity.Models
{
    public class EmailViewModel
    {
       
        [DataType(DataType.EmailAddress)]
        [Display(Name ="Nhập địa chỉ email")]
        public string Email { get; set; }   
        
    }
}