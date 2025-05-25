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
        private readonly IProductNameRepo _repository;

        private readonly ILogger<ProductInventoryController> _logger;
        private readonly IMapper _mapper;
        private readonly IProductInventoryRepo _productRepository;

        public ProductInventoryController(IProductNameRepo repository, ILogger<ProductInventoryController> logger, IMapper mapper, IProductInventoryRepo productRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _productRepository = productRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = new List<ProductName>();
            string productName = "";
            int itemShowNumber = 10;
            int existPage = 1;
            var productsReadDto = new List<ProductNameReadDto>();
            if (string.IsNullOrEmpty(productName))
            {
                products = await _repository.GetAllProductNameAsync(itemShowNumber, existPage);

                if (products != null)
                {

                    foreach (var product in products)
                    {
                        var productdto = _mapper.Map<ProductNameReadDto>(product);
                        productdto.Quantity = await _productRepository.GetQuantityByIdAsync(product.Id);
                        productsReadDto.Add(productdto);
                    }

                }
            }
            else
            {
                products = await _repository.GetAllProductNameAsync(itemShowNumber, existPage);
                if (products != null)
                {
                    foreach (var product in products)
                    {
                        var productdto = _mapper.Map<ProductNameReadDto>(product);
                        productdto.Quantity = await _productRepository.GetQuantityByIdAsync(product.Id);
                        productsReadDto.Add(productdto);
                    }
                }
            }


            return View(productsReadDto);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(ProductNameCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var product = _mapper.Map<ProductName>(dto);
            var existProductName = _repository.GetProductNameByNameAsync(dto.Name);
            if (existProductName == null)
            {
                Console.WriteLine("-->Item Name is not in the stock, need to add image");
                var result = await _repository.CreateProductNameAsync(product);
                if (result = true)
                {
                    var newProduct = await _repository.GetProductNameByNameAsync(dto.Name);
                    // add product to inventory
                    var productInventory = _mapper.Map<ProductInventory>(dto);
                    productInventory.ProductNameId = newProduct.Id;
                    var result_add = await _productRepository.CreateProductInventoryAsync(productInventory);
                    if (result_add) return RedirectToAction("AddImage", new { newProduct = newProduct });
                    Console.WriteLine("-->Can not add product");
                    return View();

                }
                else
                {
                    Console.WriteLine("-->Can not add product");
                    return View();
                }

            }

            else
            {

                var newProduct = await _repository.GetProductNameByNameAsync(dto.Name);
                // add product to inventory
                var productInventory = _mapper.Map<ProductInventory>(dto);
                productInventory.ProductNameId = newProduct.Id;
                var result_add = await _productRepository.CreateProductInventoryAsync(productInventory);
                if (result_add)
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

        [HttpGet]
        public async Task<IActionResult> DetailAsync(string productName)
        {
            if (string.IsNullOrEmpty(productName))
            {
                return NotFound("Name is null");
            }
            var product = await _repository.GetProductNameByNameAsync(productName);
            var productReadDto = new ProductNameReadDto();
            productReadDto = _mapper.Map<ProductNameReadDto>(product);
            productReadDto.Quantity = product.ProductInventories.Count();
            return View(productReadDto);


        }
        // Edit productName
        [HttpGet]
        public async Task<IActionResult> UpdateAsync(string productNameId)
        {
            if (string.IsNullOrEmpty(productNameId))
            {
                return NotFound();
            }
            var product = await _repository.GetProductNameByIdAsync(productNameId);
            var productUpdateDto = _mapper.Map<ProductNameUpdateDto>(product);
            return View(productUpdateDto);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(ProductNameUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("--> Validate error");
                return View();
            }
            var existProduct = await _repository.GetProductNameByIdAsync(dto.Id.ToString());
            if (existProduct == null)
            {
                Console.WriteLine("--> Product Nane is not existing");
                return View();

            }
            var product = _mapper.Map<ProductName>(dto);
            var result = await _repository.UpdateProductNameAsync(product);
            if (result)
            {
                return RedirectToAction("Index", new { softbyDate = true });
            }
            else
            {
                Console.WriteLine("--> Could not update product Name");
                return View();

            }
        }


    }
}