using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Models
{

    public class CartItem
    {
        public int Quantity { get; set; }
        public Product Product { get; set; }
    }
}