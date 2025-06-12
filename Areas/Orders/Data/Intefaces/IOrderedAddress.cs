using OMS_App.Areas.Orders.Models;
namespace OMS_App.Areas.Orders.Data
{
    public interface IOrderAddressRepo
    {
        Task<List<OrderAddress>> GetAllOrderAddressesByUserIdAsync(string userId);
        Task<OrderAddress> GetOrderAddressByIdAsync(string addressId);
        Task<bool> CreateAsync(OrderAddress product);
        Task<bool> UpdateAsync(OrderAddress product);
        Task<bool> DeleteAsync(OrderAddress product);
        Task<bool> SetOrderAddressToDefaultAsync(int addressId);

    }
}