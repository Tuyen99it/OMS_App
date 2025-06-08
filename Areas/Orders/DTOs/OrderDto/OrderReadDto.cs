using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Orders.Models;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Dtos
{
    public class OrderReadDto
    {
        public int Id { get; set; }

        public ICollection<OrderedProduct> OrderedProducts { get; set; }

        public List<Dictionary<OrderStatus, DateTime>> UpdateStatuses { get; set; }
        public AppUser User { get; set; }
        public double OrderedPriceTotal { get; set; }
        public OrderAddress Address { get; set; }


    }
}