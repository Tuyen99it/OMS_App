using OMS_App.Areas.Inventory.Models;
using OMS_App.Areas.Orders.Models;
namespace OMS_App.Areas.Orders.Data
{
    public interface IOrderedProductRepo
    {
        Task<List<OrderedProduct>> GetAllOrderedProductByOrderIdAsync(string orderId);
        Task<List<OrderedProduct>> GetOrderedProductsByUserIdAsync(string userId);
        
        Task<OrderedProduct> GetOrderedProductByIdAsync(string orderedProductId);
         Task<OrderedProduct> GetOrderedProductByNameAsync(string orderedProductName);
        Task<bool> CreateAsync(OrderedProduct orderedProduct);
        Task<bool> UpdateAsync(OrderedProduct orderedProduct);
        Task<bool> DeleteAsync(OrderedProduct orderedProduct);

    }
}