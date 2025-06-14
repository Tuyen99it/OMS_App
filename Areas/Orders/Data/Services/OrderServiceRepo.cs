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
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders.Include(o => o.OrderStatusUpdates).Include(o => o.OrderedProducts).ToListAsync();

            return orders;
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(string orderStatus)
        {
            var status = new OrderStatus();
            switch (orderStatus)
            {
                case "Create":
                    // Add logic for "Create" status here if needed
                    status = OrderStatus.Create;
                    break;
                case "Send":
                    // Add logic for "Create" status here if needed
                    status = OrderStatus.Send;
                    break;
                case "Delivery":
                    // Add logic for "Create" status here if needed
                    status = OrderStatus.Delivery;
                    break;
                case "Paid":
                    // Add logic for "Create" status here if needed
                    status = OrderStatus.Paid;
                    break;
                case "Finish":
                    // Add logic for "Create" status here if needed
                    status = OrderStatus.Finish;
                    break;
                case "Cancel":
                    // Add logic for "Create" status here if needed
                    status = OrderStatus.Cancel;
                    break;

            }
            var orders = await _context.Orders.Where(o => o.OrderStatusUpdates.Any(u => u.Status == status && u.IsStatusUpdate == true))
                .Include(o => o.OrderStatusUpdates)
                .Include(o => o.OrderedProducts)
                .ToListAsync();

            return orders;
        }
        public async Task<List<Order>> GetAllOrdersByUserIdAsync(string userId)
        {
            var orders = await _context.Orders.Where(o => o.User.Id == userId).Include(o => o.OrderStatusUpdates).Include(o => o.OrderedProducts).ToListAsync();

            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
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

        public async Task<List<Order>> GetAllOrdersByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("-->UserId is null");
                return null;
            }
            return await _context.Orders.Where(o => o.User.Id == userId).ToListAsync();
        }



        public async Task<bool> UpdateAsync(Order order)
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



        public async Task<bool> CreateOrderStatusAsync(OrderStatusUpdate status)
        {
            if (status == null)
            {
                Console.WriteLine("-->Status  is null");
                return false;

            }
            _context.OrderStatusUpdates.Add(status);
            _context.SaveChanges();
            return true;
        }

        public async Task<Order> GetNewestOrderAsync()
        {
            // Get all newOrder
            var newOrder = await _context.Orders.Where(o => o.IsNewCreate == true).FirstOrDefaultAsync();
            // reset isNewestOrder
            if (newOrder == null)
            {
                return null;
            }
            newOrder.IsNewCreate = false;
            _context.Orders.Update(newOrder);
            _context.SaveChanges();
            return newOrder;
        }
        public async Task<List<OrderStatusUpdate>> GetOrderStatusByOrderIdAsync(string orderId)
        {
            return  await _context.OrderStatusUpdates.Where(u => u.OrderId == Convert.ToInt32(orderId)).ToListAsync();
            
        }

       
    }
}