using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Models
{
    public class OrderUpdateDto
    {
        public Dictionary<OrderStatus, DateTime> UpdateStatus { get; set; }


    }
}