using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Models
{
    public class OrderAddress
    {
        [Key]
        public int Id { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Locality { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressDescription { get; set; }
        public bool isDefault { get; set; } = false;


        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        public int? OrderId { get; set; }
        public Order Order { get; set; }

       

      
    }

}
