using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Models
{
    public class OrderedProductCreateDto
    {
        public string ProductName { get; set; }
        public int TotalProduct { get; set; }
        public double Price { get; set; }
        public double TotalPrices { get; set; }
        public int OrderId { get; set; }


    }
}