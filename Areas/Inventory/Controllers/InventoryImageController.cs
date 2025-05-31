using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using OMS_App.Areas.Inventory.Data;
using OMS_App.Areas.Inventory.Dtos;
using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class InventtoryImageController : Controller
    {
        private readonly IInventoryImageRepo _repository;
        private readonly ILogger<CategoryProductController> _logger;
        private readonly IMapper _mapper;
        public InventtoryImageController(IInventoryImageRepo repository, ILogger<CategoryProductController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImageAsync(IFormFile files, InventoryImage image)
        {
            if (files == null || files.Length == 0 || image == null)
            {
                Console.WriteLine("-->No file uploaded");
                return Json(new { success = false, message = "No file uploaded" });
            }
            if (!Directory.Exists(image?.AbsoluteImageUrlPath))
            {
                Directory.CreateDirectory(image.AbsoluteImageUrlPath);
            }
            // Get name of File
            var uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(files.FileName);
            var relativeFilePath = Path.Combine(image.RelativeImageUrlPath, uniqueFilename);
            var absoluteFilePath = Path.Combine(image.AbsoluteImageUrlPath, uniqueFilename);
            image.RelativeImageUrlPath = relativeFilePath;
            image.AbsoluteImageUrlPath = absoluteFilePath;
            try
            {
                var result = await _repository.CreateImageAsync(image);
                if (result)
                {
                    using (var filestream = new FileStream(absoluteFilePath, FileMode.Create))
                    {
                        await files.CopyToAsync(filestream);
                    }
                    Console.WriteLine("Upload success");
                    return Json(new { success = "true", filePath = relativeFilePath });


                }
                else
                {
                    Console.WriteLine("Upload faile");
                    return Json(new { success = "true", message = "upload file fail" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return Json(new { success = false, message = "An error occurred while uploading the file." });
            }
        }
        // Edit productName


        [HttpGet]
        public async Task<IActionResult> DeleteAsync(string imageId)
        {
            if (string.IsNullOrEmpty(imageId))
            {
                return NotFound("ImageId is null");
            }
            var existingImage = await _repository.GetImageByIdAsync(imageId);
            if (existingImage == null)
            {
                Console.WriteLine("-->Image is not existing");
                return NotFound("Image is not existing");
            }
            try
            {
                if (!System.IO.File.Exists(existingImage.AbsoluteImageUrlPath))
                {
                    Console.WriteLine("-->Image is not existing in the Image store");
                    return Json(new { success = false, message = "Image is not existing in the Image store" });

                }

                var result = await _repository.DeleteImageAsync(existingImage);
                if (result)
                {
                    System.IO.File.Delete(existingImage.AbsoluteImageUrlPath);
                    Console.WriteLine("Delete success");
                    return Json(new { success = "true", message = "Image deleted" });
                }
                else
                {
                    Console.WriteLine("Delete success");
                    return Json(new { success = false, message = "Image delete fail" });

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return Json(new { success = false, message = "An error occurred while delete the file." });
            }
        }
    }
}