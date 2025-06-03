using AspNetCoreGeneratedDocument;
using Microsoft.EntityFrameworkCore;
using OMS_App.Areas.Orders.Data;
using OMS_App.Areas.Orders.Models;
using OMS_App.Data;

namespace OMS_App.Areas.Orders.Data
{
    public class OrdereServiceRepo : IOrderRepo
    {
        private readonly OMSDBContext _context;
        private readonly ILogger<OrdereServiceRepo> _logger;

        public OrdereServiceRepo(OMSDBContext context, ILogger<OrdereServiceRepo> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<bool> CreateAsync(OMS_App.Areas.Orders.Models.Order order)
        {
            if (order == null)
            {
                _logger.LogError("order is null");
                return false;
            }
            var existorder = await _context.Orders.AnyAsync(c => c.Id == order.Id);
            if (existorder)
            {
                _logger.LogWarning("order is not existing");
                return false;
            }
            _context.Orders.Add(order);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> DeleteAsync(OMS_App.Areas.Orders.Models.Order order)
        {
            if (order == null)
            {
                _logger.LogError("order is null");
                return false;
            }
            var existorder = await _context.Orders.AnyAsync(c => c.Id == order.Id);
            if (!existorder)
            {
                _logger.LogWarning("order is not existing");
                return false;
            }
            _context.Orders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async  Task<List<Order>> GetAllOrdersByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("-->UserId is null");
                return null;
            }
            return await _context.Orders.Where(o => o.User.Id == userId).ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersByUserIdAsync(string userId)
        {
            var orders = await _context.Orders.Where(o => o.User.Id == userId).Include(o => o.OrderStatusUpdates).Include(o=>o.OrderedProducts).ToListAsync();

            return orders;
        }

        public async Task<OMS_App.Areas.Orders.Models.Order> GetOrderByIdAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                _logger.LogError("order is Empty");
                return null;
            }
            var order = _context.Orders
                                   .Where(o => o.Id == Convert.ToInt16(orderId))
                                   .Include(o => o.OrderedProducts).Include(o => o.OrderStatusUpdates)
                                   .FirstOrDefault();
            return order;
        }

        public async Task<bool> UpdateAsync(OMS_App.Areas.Orders.Models.Order order)
        {
            if (order == null)
            {
                _logger.LogError("order is null");
                return false;
            }
            var existorder = _context.Orders.Where(c => c.Id == order.Id).FirstOrDefault();
            if (existorder == null)
            {
                _logger.LogError("There is no order in the database");
                return false;

            }
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}