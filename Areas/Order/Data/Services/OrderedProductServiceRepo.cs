using Microsoft.EntityFrameworkCore;
using OMS_App.Areas.Orders.Models;
using OMS_App.Areas.Orders.Data;
using OMS_App.Data;

namespace OMS_App.Areas.Orders.Data
{
    public class OrderedProductServiceRepo : IOrderedProductRepo
    {
        private readonly OMSDBContext _context;
        private readonly ILogger<OrderedProductServiceRepo> _logger;
        public OrderedProductServiceRepo(OMSDBContext context, ILogger<OrderedProductServiceRepo> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> CreateAsync(OrderedProduct product)
        {
            if (product == null)
            {
                throw new NullReferenceException("Product is null");
            }
            var existProduct = await _context.OrderedProducts.AnyAsync(p => p.Id == product.Id);
            if (existProduct)
            {
                _logger.LogInformation("Product is existing");
                return false;
            }
            await _context.OrderedProducts.AddAsync(product);
            _logger.LogInformation("Product is created ");
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteAsync(OrderedProduct product)
        {

            if (product == null)
            {
                throw new NullReferenceException("Product is null");
            }
            var existProduct = await _context.ProductInventories.AnyAsync(p => p.Id == product.Id);
            if (!existProduct)
            {
                _logger.LogInformation("Product is not existing");
                return false;
            }
            _context.OrderedProducts.Remove(product);
            _logger.LogInformation("Product is deleted ");
            return _context.SaveChanges() > 0;
        }
         public async Task<List<OrderedProduct>> GetAllOrderedProductByOrderIdAsync(string orderId)
        {
            return await _context.OrderedProducts.Where(p => p.Order.Id == Convert.ToInt32(orderId)).ToListAsync();
        }

       

        public async Task<OrderedProduct> GetOrderedProductByIdAsync(string productId)
        {
            if (String.IsNullOrEmpty(productId))
            {
                throw new NullReferenceException("Product is null");
            }
            var product = await _context.OrderedProducts.Where(p => p.Id == Convert.ToInt32(productId)).FirstOrDefaultAsync();

            return product;
        }

        public async Task<OrderedProduct> GetOrderedProductByNameAsync(string orderedProductName)
        {
            if (String.IsNullOrEmpty(orderedProductName))
            {
                throw new NullReferenceException("Product is null");
            }
            var product = await _context.OrderedProducts.Where(p => p.ProductName.Contains(orderedProductName)).FirstOrDefaultAsync();
            return product;
        }

        public async Task<bool> UpdateAsync(OrderedProduct product)
        {
            if (product == null)
            {
                throw new NullReferenceException("Product is null");
            }
            var existProduct = await _context.ProductInventories.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (existProduct == null)
            {
                _logger.LogInformation("Product not found for update");
                return false;
            }
            _context.Entry(existProduct).CurrentValues.SetValues(product);
            _logger.LogInformation("Product is updated");
            return await _context.SaveChangesAsync() > 0;
        }


    }
}