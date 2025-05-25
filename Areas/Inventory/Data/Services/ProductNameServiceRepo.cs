using Microsoft.EntityFrameworkCore;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Data;

namespace OMS_App.Areas.Inventory.Data
{
    public class ProductNameServiceRepo : IProductNameRepo
    {
        private readonly OMSDBContext _context;
        private readonly ILogger<ProductNameServiceRepo> _logger;
        public ProductNameServiceRepo(OMSDBContext context, ILogger<ProductNameServiceRepo> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> CreateProductNameAsync(ProductName product)
        {
            if (product == null)
            {
                throw new NullReferenceException("Product is null");
            }
            var existProduct = await _context.ProductNames.AnyAsync(p => p.Id == product.Id);
            if (existProduct)
            {
                _logger.LogInformation("Product is existing");
                return false;
            }
            await _context.ProductNames.AddAsync(product);
            _logger.LogInformation("Product is created ");
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteProductNameAsync(ProductName product)
        {

            if (product == null)
            {
                throw new NullReferenceException("Product is null");
            }
            var existProduct = await _context.ProductNames.AnyAsync(p => p.Id == product.Id);
            if (!existProduct)
            {
                _logger.LogInformation("Product is not existing");
                return false;
            }
            _context.ProductNames.Remove(product);
            _logger.LogInformation("Product is deleted ");
            return _context.SaveChanges() > 0;
        }

        public async Task<List<ProductName>> GetAllProductNameAsync(int itemShowNumber, int existPage)
        {

            itemShowNumber = (itemShowNumber <= 0) ? 10 : itemShowNumber;

            var totalItem = _context.ProductNames.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / itemShowNumber);
            totalPage = (totalPage <= 0) ? 1 : totalPage;


            if (existPage <= 0) existPage = 1;
            if (existPage >= totalPage) existPage = totalPage;
            return await _context.ProductNames.Skip((existPage - 1) * itemShowNumber)
                                                .Take(itemShowNumber)
                                                .Include(p => p.ProductImages)
                                               .Include(p => p.CategoriesProduct)
                                               .ThenInclude(c => c.ProductCategory).ToListAsync();

        }

        public async Task<List<ProductName>> GetProductNameByNameAsync(string searchName, int itemShowNumber, int existPage)
        {

            itemShowNumber = (itemShowNumber <= 0) ? 1 : itemShowNumber;
            var totalItem = _context.ProductNames.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / itemShowNumber);
            totalPage = (totalPage <= 0) ? 1 : totalPage;
            // Calculate skip, but never less than 0
            int skip = Math.Max(0, (existPage - 1) * itemShowNumber);
            if (existPage <= 0) existPage = 1;
            if (existPage >= totalPage) existPage = totalPage;
            return await _context.ProductNames.Where(p => p.Name.Contains(searchName))
                                                    .Skip(skip)
                                                    .Take(itemShowNumber)
                                                    .Include(p => p.ProductImages)
                                                    .Include(p => p.CategoriesProduct)
                                                    .ThenInclude(c => c.ProductCategory).ToListAsync();

        }

        public async Task<ProductName> GetProductNameByIdAsync(string productId)
        {
            if (String.IsNullOrEmpty(productId))
            {
                throw new NullReferenceException("Product is null");
            }
            var product = await _context.ProductNames.Where(p => p.Id == Convert.ToInt32(productId))
                                                           .Include(p => p.ProductImages)
                                                           .Include(p => p.CategoriesProduct)
                                                           .ThenInclude(c => c.ProductCategory)
                                                           .FirstOrDefaultAsync();

            return product;
        }
        public async Task<ProductName> GetProductNameByNameAsync(string productName)
        {
            if (String.IsNullOrEmpty(productName))
            {
                throw new NullReferenceException("Product name is null");
            }
            var products = await _context.ProductNames.Where(p => p.Name.Contains(productName)).ToListAsync();
            var product = products.FirstOrDefault();
            product.Quantity = products.Count();
            return product;

        }
        public async Task<bool> UpdateProductNameAsync(ProductName product)
        {
            if (product == null)
            {
                throw new NullReferenceException("Product is null");
            }
            var existProduct = await _context.ProductNames.FirstOrDefaultAsync(p => p.Id == product.Id);
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