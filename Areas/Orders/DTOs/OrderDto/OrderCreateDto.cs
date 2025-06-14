using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Models
{
    public class OrderCreateDto
    {
        public ICollection<OrderedProduct> OrderedProducts { get; set; }
        public double OrderedPriceTotal { get; set; }

    }

}