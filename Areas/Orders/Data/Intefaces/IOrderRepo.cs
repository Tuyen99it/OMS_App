using OMS_App.Areas.Orders.Models;
namespace OMS_App.Areas.Orders.Data
{
  public interface IOrderRepo
  {
    Task<List<Order>> GetAllOrdersAsync();
    Task<List<Order>> GetOrdersByStatusAsync(string orderStatus);
    Task<List<Order>> GetAllOrdersByUserIdAsync(string userId);

    Task<Order> GetOrderByIdAsync(string orderId);
    Task<bool> CreateAsync(Order order);
    Task<bool> UpdateAsync(Order order);
    Task<bool> DeleteAsync(Order order);

    Task<bool> CreateOrderStatusAsync(OrderStatusUpdate status);
    Task<Order> GetNewestOrderAsync();
    Task<List<OrderStatusUpdate>> GetOrderStatusByOrderIdAsync(string orderId);
    }
}