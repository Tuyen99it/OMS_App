using Microsoft.EntityFrameworkCore;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Data;

namespace OMS_App.Areas.Inventory.Data
{
    public class ProductInventoryServiceRepo : IProductInventoryRepo
    {
        private readonly OMSDBContext _context;
        private readonly ILogger<ProductInventoryServiceRepo> _logger;
        public ProductInventoryServiceRepo(OMSDBContext context, ILogger<ProductInventoryServiceRepo> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> CreateProductInventoryAsync(ProductInventory product)
        {
            if (product == null)
            {
                throw new NullReferenceException("Product is null");
            }
            var existProduct = await _context.ProductInventories.AnyAsync(p => p.Id == product.Id);
            if (existProduct)
            {
                _logger.LogInformation("Product is existing");
                return false;
            }
            await _context.ProductInventories.AddAsync(product);
            _logger.LogInformation("Product is created ");
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteProductInventoryAsync(ProductInventory product)
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
            _context.ProductInventories.Remove(product);
            _logger.LogInformation("Product is deleted ");
            return _context.SaveChanges() > 0;
        }

        public async Task<List<ProductInventory>> GetAllProductInventoryAsync(int itemShowNumber, int existPage)
        {

            itemShowNumber = (itemShowNumber <= 0) ? 1 : itemShowNumber;
            var totalItem = _context.ProductInventories.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / itemShowNumber);
            if (existPage <= 0) existPage = 1;
            if (existPage >= totalPage) existPage = totalPage;
            return await _context.ProductInventories.Skip((existPage - 1) * itemShowNumber)
                                                    .Take(itemShowNumber)
                                                    .ToListAsync();

        }


        public async Task<ProductInventory> GetProductInventoryByIdAsync(string productId)
        {
            if (String.IsNullOrEmpty(productId))
            {
                throw new NullReferenceException("Product is null");
            }
            var product = await _context.ProductInventories.Where(p => p.Id == Convert.ToInt32(productId)).FirstOrDefaultAsync();

            return product;
        }

        public async Task<int> GetQuantityByIdAsync(int productId)
        {
            return await _context.ProductInventories.Where(p => p.ProductNameId == productId).CountAsync();
        }

        public async Task<bool> UpdateProductInventoryAsync(ProductInventory product)
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