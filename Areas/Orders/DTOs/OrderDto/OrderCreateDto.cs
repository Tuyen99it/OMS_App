using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Orders.Models;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Dtos
{
    public class OrderCreateDto
    {

        public int[] OrderedProductsId { get; set; }
        public AppUser User { get; set; }
        public OrderAddress Address { get; set; }
    

    }
}