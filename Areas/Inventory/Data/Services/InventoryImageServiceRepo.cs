using Microsoft.EntityFrameworkCore;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Data;

namespace OMS_App.Areas.Inventory.Data
{
    public class InventoryImageServiceRepo : IInventoryImageRepo
    {
        private readonly OMSDBContext _context;
        private readonly ILogger<InventoryImageServiceRepo> _logger;

        public InventoryImageServiceRepo(OMSDBContext context, ILogger<InventoryImageServiceRepo> logger)
        {
            _context = context;
            _logger = logger;

        }
        public async Task<bool> CreateImageAsync(InventoryImage image)
        {
            if (image == null)
            {
                _logger.LogError("Image is null");
                return false;
            }
            var existsImage = _context.InventoryImages.Any(i => i.Id == image.Id);
            if (existsImage)
            {
                _logger.LogError("Image is existing");
                return false;
            }
            await _context.InventoryImages.AddAsync(image);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> DeleteImageAsync(InventoryImage image)
        {
            if (image == null)
            {
                _logger.LogError("Image is null");
                return false;
            }
            var existsImage = _context.InventoryImages.Any(i => i.Id == image.Id);
            if (!existsImage)
            {
                _logger.LogError("Image is not existing");
                return false;
            }
            _context.InventoryImages.Remove(image);
            return await _context.SaveChangesAsync() > 0;
            
        }

        public async Task<List<InventoryImage>> GetAllImages(string imageTypeId, ImageType type)
        {
            if (string.IsNullOrEmpty(imageTypeId))
            {
                _logger.LogError("Image is null");
                return null;
            }
            if (type == ImageType.ProductInventory)
            {
                var images = await _context.InventoryImages.Where(i => i.ProductInventoryId == Convert.ToInt32(imageTypeId)).ToListAsync();
                return images;
            }
            else
            {
                var images = await _context.InventoryImages.Where(i => i.ProductCategoryId == Convert.ToInt32(imageTypeId)).ToListAsync();
                return images;
                
            }
          
            
        }

        public async Task<bool> UpdateImageAsync(InventoryImage image)
        {
            if (image == null)
            {
                _logger.LogError("Image is null");
                return false;
            }
            var existsImage = _context.InventoryImages.FirstOrDefault(i => i.Id == image.Id);
            if (existsImage==null)
            {
                _logger.LogError("Image is not existing");
                return false;
            }
            _context.InventoryImages.Update(image);
            return await _context.SaveChangesAsync() > 0;
           
        }
    }
}