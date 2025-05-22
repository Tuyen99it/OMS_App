using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OMS_App.Areas.Inventory.Data;
using OMS_App.Areas.Inventory.Dtos;
using OMS_App.Areas.Inventory.Models;
namespace OMS_App.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class ProductInventoryController : Controller
    {
        private readonly IProductInventoryRepo _repository;
        private readonly ILogger<ProductInventoryController> _logger;
        private readonly IMapper _mapper;

        public ProductInventoryController(IProductInventoryRepo repository, ILogger<ProductInventoryController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<IActionResult> Index(string? productName, int itemShowNumber, int existPage)
        {
            var products = new List<ProductInventory>();
            if (string.IsNullOrEmpty(productName))
            {
                products = await _repository.GetAllProductInventoryAsync(itemShowNumber, existPage);
            }
            else
            {
                products = await _repository.GetProductsInventoryByNameAsync(productName, itemShowNumber, existPage);
            }


            return View(_mapper.Map<IEnumerable<InventoryProductReadDto>>(products));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(InventoryProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var product = _mapper.Map<ProductInventory>(dto);
            var existProductInventory = _repository.GetProductInventoryByNameAsync(dto.Name);
            if (existProductInventory == null)
            {
                Console.WriteLine("-->Item Name is not in the stock, need to add image");
                var result = await _repository.CreateProductInventoryAsync(product);
                if (result = true)
                {
                    var newProduct = await _repository.GetProductInventoryByNameAsync(dto.Name);
                    return RedirectToAction("AddImage", new { newProduct = newProduct });

                }
                else
                {
                    Console.WriteLine("-->Can not add product");
                    return View();
                }

            }

            else
            {
                var result = await _repository.CreateProductInventoryAsync(product);
                if (result = true)
                {
                    return RedirectToAction("Index", new { softbyDate = true });

                }
                else
                {
                    Console.WriteLine("-->Can not add product");
                    return View();
                }
            }
        }


    }
}