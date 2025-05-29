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
    public class ProductInventoryController : Controller
    {
        private readonly IProductNameRepo _repository;
        private readonly ILogger<ProductInventoryController> _logger;
        private readonly IMapper _mapper;
        private readonly IProductInventoryRepo _productRepository;
        public List<ProductNameCreateDto> Products { get; set; } = new();
        public ProductInventoryController(IProductNameRepo repository, ILogger<ProductInventoryController> logger, IMapper mapper, IProductInventoryRepo productRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _productRepository = productRepository;
        }
 
        public async Task<IActionResult> Index()
        {
             Console.WriteLine("--> Come to Index get");
            ProductNameIndexViewModel model = new();
          
            int itemShowNumber = 10;
            int existPage = 1;
            var productsReadDto = new List<ProductNameReadDto>();

            var products = await _repository.GetAllProductNameAsync(itemShowNumber, existPage);

            if (products != null)
            {

                foreach (var product in products)
                {
                    var productdto = _mapper.Map<ProductNameReadDto>(product);
                    productdto.Quantity = await _productRepository.GetQuantityByIdAsync(product.Id);
                    productsReadDto.Add(productdto);
                }
                model.ProductsNameReadDto = productsReadDto;
                return View(model);

            }
      
            return View();
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProductNameIndexViewModel? model)
        {
            Console.WriteLine("--> Come to Index post");
            var newModel = new ProductNameIndexViewModel();
            var products = new List<ProductName>();

            int itemShowNumber = 10;
            int existPage = 1;
            var productsReadDto = new List<ProductNameReadDto>();
            if (string.IsNullOrEmpty(model.SearchName))
            {
                Console.WriteLine("--> No search name");
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
                Console.WriteLine("--> Enter Search Name controller");
                products = await _repository.GetProductNameByNameAsync(model.SearchName, itemShowNumber, existPage);
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
            newModel.ProductsNameReadDto = productsReadDto;

            return View(newModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProductInventoryViewModel();


            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(int number, ProductInventoryViewModel model)
        {
            var newModel = new ProductInventoryViewModel();

            Console.WriteLine("-->Start create product");
            // if (!ModelState.IsValid)
            // {
            //     Console.WriteLine("-->Input invalid");
            //     return View();
            // }
            Console.WriteLine(number);
            var product = _mapper.Map<ProductName>(model.ProductNameCreateDto);
            var existProductName = await _repository.GetProductNameByNameAsync(model.ProductNameCreateDto.Name);
            if (existProductName == null)
            {
                Console.WriteLine("-->Item Name is not in the stock, need to add image");

                var result = await _repository.CreateProductNameAsync(product);
                if (result == true)
                {   // Get product follow number
                    var productsTemp = await _repository.GetLastProductsByNumberAsync(number + 1);

                    var productNameReadDto = _mapper.Map<List<ProductNameReadDto>>(productsTemp);
                    newModel.ProductsNameReadDto = productNameReadDto;
                    newModel.ProductNameCreateDto = null;
                    newModel.number = number + 1;
                    return View(newModel);
                }
                else
                {
                    Console.WriteLine("-->Can not add product");
                    return View(Products);
                }

            }

            else
            {
                Console.WriteLine("--> Product Name is existing");
                return View(Products);
            }

        }

        [HttpGet]
        public async Task<IActionResult> DetailAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return NotFound("ProductId is null");
            }
            var product = await _repository.GetProductNameByIdAsync(productId);
            var productReadDto = _mapper.Map<ProductNameReadDto>(product);

            return View(productReadDto);


        }
        // Edit productName
        [HttpGet]
        public async Task<IActionResult> UpdateAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return NotFound();
            }
            var product = await _repository.GetProductNameByIdAsync(productId);
            var productUpdateDto = _mapper.Map<ProductNameUpdateDto>(product);
            return View(productUpdateDto);
        }

        [HttpPost]
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
                Console.WriteLine("--> Product Id is not existing");
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

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return NotFound();
            }
            var product = await _repository.GetProductNameByIdAsync(productId);

            var result = await _repository.DeleteProductNameAsync(product);
            if (result)
            {
                return RedirectToAction("Index", new { softbyDate = true });
            }
            else
            {
                Console.WriteLine("--> Could not delete product Name");
                return View();

            }
        }



    }
}