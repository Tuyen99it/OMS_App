using Microsoft.EntityFrameworkCore;

using OMS_App.Areas.Orders.Models;
using OMS_App.Data;

namespace OMS_App.Areas.Orders.Data
{
    public class OrderAddressServiceRepo : IOrderAddressRepo
    {
        private readonly OMSDBContext _context;
        private readonly ILogger<OrderAddressServiceRepo> _logger;
        public OrderAddressServiceRepo(OMSDBContext context, ILogger<OrderAddressServiceRepo> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<OrderAddress>> GetAllOrderAddressesByUserIdAsync(string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("-->UserId is null");
                return null;
            }
            var orderAddresses = await _context.OrderAddresses.Where(a => a.User.Id == userId).ToListAsync();
            return orderAddresses;

        }

        public async Task<OrderAddress> GetOrderAddressByIdAsync(string addressId)
        {
            if (String.IsNullOrEmpty(addressId))
            {
                throw new NullReferenceException("address is null");
            }
            var address = await _context.OrderAddresses.Where(p => p.Id == Convert.ToInt32(addressId)).FirstOrDefaultAsync();
            return address;
        }
        public async Task<bool> CreateAsync(OrderAddress address)
        {
            if (address == null)
            {
                throw new NullReferenceException("address is null");
            }
            var existaddress = await _context.OrderAddresses.AnyAsync(p => p.Id == address.Id);
            if (existaddress)
            {
                _logger.LogInformation("address is existing");
                return false;
            }
            await _context.OrderAddresses.AddAsync(address);
            _logger.LogInformation("address is created ");
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteAsync(OrderAddress address)
        {

            if (address == null)
            {
                throw new NullReferenceException("address is null");
            }
            var existaddress = await _context.OrderAddresses.AnyAsync(p => p.Id == address.Id);
            if (!existaddress)
            {
                _logger.LogInformation("address is not existing");
                return false;
            }
            _context.OrderAddresses.Remove(address);
            _logger.LogInformation("address is deleted ");
            return _context.SaveChanges() > 0;
        }


        public async Task<bool> UpdateAsync(OrderAddress address)
        {
            if (address == null)
            {
                throw new NullReferenceException("address is null");
            }
            var existaddress = await _context.OrderAddresses.FirstOrDefaultAsync(p => p.Id == address.Id);
            if (existaddress == null)
            {
                _logger.LogInformation("address not found for update");
                return false;
            }
            _context.Entry(existaddress).CurrentValues.SetValues(address);
            _logger.LogInformation("address is updated");
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetOrderAddressToDefaultAsync(int addressId)
        {
            // get existing default 
            var existingDefault = _context.OrderAddresses.Where(a => a.isDefault == true).FirstOrDefault();
            existingDefault.isDefault = false;
            // get address
            var setDefautAddress = _context.OrderAddresses.Where(a => a.Id == addressId).FirstOrDefault();
            setDefautAddress.isDefault = true;

            // reset existing default
            _context.OrderAddresses.Update(existingDefault);
            _context.OrderAddresses.Update(setDefautAddress);
            return await _context.SaveChangesAsync() > 0;

        }

      
    }
}