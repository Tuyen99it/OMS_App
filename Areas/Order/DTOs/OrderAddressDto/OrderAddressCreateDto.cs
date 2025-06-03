using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Dtos
{
    public class OrderAddressCreateDto
    {
        public string Country { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Locality { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressDescription { get; set; }

    }

}