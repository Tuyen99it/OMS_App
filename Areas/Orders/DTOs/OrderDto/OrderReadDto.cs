using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Orders.Models;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Dtos
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public bool IsNewCreate { get; set; } 
        public double OrderedPriceTotal { get; set; }


        public ICollection<OrderedProduct> OrderedProducts { get; set; }

        public ICollection<OrderStatusUpdate> OrderStatusUpdates { get; set; }
      
    }
}