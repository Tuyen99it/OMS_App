using OMS_App.Areas.Inventory.Models;
using OMS_App.Areas.Orders.Models;
namespace OMS_App.Areas.Orders.Data
{
    public interface IOrderedProductRepo
    {
        Task<List<OrderedProduct>> GetAllOrderedProductByOrderIdAsync(string orderId);
        Task<Order> GetOrderedProductByIdAsync(string orderedProductId);
        Task<bool> CreateAsync(OrderedProduct orderedProduct);
        Task<bool> UpdateAsync(OrderedProduct orderedProduct);
        Task<bool> DeleteAsync(OrderedProduct orderedProduct);

    }
}