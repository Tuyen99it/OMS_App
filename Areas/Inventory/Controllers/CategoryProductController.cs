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
    public class CategoryProductController : Controller
    {
        private readonly IProductCategoryRepo _repository;
        private readonly ILogger<CategoryProductController> _logger;
        private readonly IMapper _mapper;
        private readonly IProductInventoryRepo _productRepository;
        public List<ProductNameCreateDto> Products { get; set; } = new();
        public CategoryProductController(IProductCategoryRepo repository, ILogger<CategoryProductController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }

        public async Task<IActionResult> Index()
        {
            Console.WriteLine("--> Come to Index get");
            var model = new CategoryProductIndexViewModel();
            var products = await _repository.GetAllProductCategoriesAsync();

            if (products != null)
            {
                model.CategoriesProductReadDto = _mapper.Map<List<ProductCategoryReadDto>>(products);
                return View(model);

            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CategoryProductIndexViewModel? model)
        {
            Console.WriteLine("--> Come to Index post");
            var newModel = new CategoryProductIndexViewModel();
            if (string.IsNullOrEmpty(model.SearchName))
            {
                Console.WriteLine("--> No search name");
                var products = await _repository.GetAllProductCategoriesAsync();

                if (products != null)
                {
                    newModel.CategoriesProductReadDto = _mapper.Map<List<ProductCategoryReadDto>>(products);
                }
                return View(newModel);
            }
            else
            {
                Console.WriteLine("--> Enter Search Name controller");
                var products = await _repository.GetCategoriesProductByNameAsync(model.SearchName);
                if (products != null)
                {
                    newModel.CategoriesProductReadDto = _mapper.Map<List<ProductCategoryReadDto>>(products);
                }
                return View(newModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CategoryProductViewModel model)
        {
            var newModel = new CategoryProductViewModel();

            Console.WriteLine("-->Start create category");
            // if (!ModelState.IsValid)
            // {
            //     Console.WriteLine("-->Input invalid");
            //     return View();
            // }

            var category = _mapper.Map<ProductCategory>(model.CategoryProductCreateDto);
            var existProductName = await _repository.GetCategoryProductByNameAsync(model.CategoryProductCreateDto.Name);
            if (existProductName == null)
            {
                Console.WriteLine("-->Category is not existing");

                var result = await _repository.CreateAsync(category);
                if (result == true)
                {
                    return View("Index", new { sortUpdate = "desceding" });
                }
                else
                {
                    Console.WriteLine("-->Can not add product");
                    return View(Products);
                }

            }

            else
            {
                Console.WriteLine("--> Category Name is existing");
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> DetailAsync(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return NotFound("ProductId is null");
            }
            var category = await _repository.GetCategoryProductByIdAsync(categoryId);
            var productReadDto = _mapper.Map<ProductCategoryReadDto>(category);

            return View(productReadDto);


        }
        // Edit productName
        [HttpGet]
        public async Task<IActionResult> UpdateAsync(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return NotFound();
            }
            var category = await _repository.GetCategoryProductByIdAsync(categoryId);
            var categoryUpdateDto = _mapper.Map<ProductCategoryUpdateDto>(category);
            return View(categoryUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(ProductCategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("--> Validate error");
                return View();
            }
            var existProduct = await _repository.GetCategoryProductByIdAsync(dto.Id.ToString());
            if (existProduct == null)
            {
                Console.WriteLine("--> Category Id is not existing");
                return View();

            }
            var category = _mapper.Map<ProductCategory>(dto);
            var result = await _repository.UpdateAsync(category);
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
        public async Task<IActionResult> DeleteAsync(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return NotFound();
            }
            var category = await _repository.GetCategoryProductByIdAsync(categoryId);

            var result = await _repository.DeleteAsync(category);
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