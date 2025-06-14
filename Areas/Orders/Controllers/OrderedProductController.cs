using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using OMS_App.Areas.Inventory.Data;
using OMS_App.Areas.Orders.Data;
using OMS_App.Areas.Orders.Dtos;
using OMS_App.Areas.Orders.Models;
using OMS_App.Models;
using System.Security.Claims;
namespace OMS_App.Areas.Orders.Controllers
{
    [Area("Orders")]
    public class OrderedProductController : Controller
    {
        private readonly IOrderedProductRepo _repository;
        private readonly ILogger<OrderedProductController> _logger;
        private readonly IMapper _mapper;
        private readonly IProductNameRepo _productNameRepository;
        private readonly UserManager<AppUser> _userManager;

        public OrderedProductController(IOrderedProductRepo repository, ILogger<OrderedProductController> logger, IMapper mapper, IProductNameRepo productNameRepository, UserManager<AppUser> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _productNameRepository = productNameRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string userId)
        {
            var model = new OrderedProductIndexViewModel();

            Console.WriteLine("--> Come to Index get");
            var products = await _repository.GetOrderedProductsByUserIdAsync(userId);

            if (products != null)
            {
                var productsReadDto = _mapper.Map<List<OrderedProductReadDto>>(products);
                model.OrderedProductsReadDto = productsReadDto;
                return View(model);

            }
            Console.WriteLine("--> There is no product");
            return View();

        }

        [HttpGet]

        public async Task<IActionResult> CreateAsync(string productId)
        {
            var product = await _productNameRepository.GetProductNameByIdAsync(productId);
            if (product == null)
            {
                Console.WriteLine("--> Could not load product information");
            }
            var orderedProduct = await _repository.GetOrderedProductByNameAsync(product.Name);
            if (orderedProduct == null||orderedProduct?.IsOrder==true)
            {

                var newOrderedProduct = _mapper.Map<OrderedProduct>(product);
                newOrderedProduct.UserId = GetExistingUserId();

                newOrderedProduct.TotalProduct = 1;
                newOrderedProduct.TotalPrices = newOrderedProduct.Price * newOrderedProduct.TotalProduct;
                
                    // Create a new ordered product
                    var result = await _repository.CreateAsync(newOrderedProduct);
                    if (!result)
                    {
                       
                        Console.WriteLine("--> Could not create new orderProduct");
                        return View();
                    }
                Console.WriteLine("-->Create a new Ordered product");
                    return RedirectToAction("Index", new { userId = GetExistingUserId() });

            }
            orderedProduct.Price = product.Price;
            orderedProduct.TotalProduct += 1;
            orderedProduct.TotalPrices = orderedProduct.Price * orderedProduct.TotalProduct;
            var updateResult = await _repository.UpdateAsync(orderedProduct);
            if (!updateResult)
            {
                Console.WriteLine("--> Could not update orderProduct");
                return View();
            }
       
        
            return RedirectToAction("Index", new { userId = GetExistingUserId() });


        }
         [HttpGet]
        public async Task<IActionResult> UpdateAsync(string productId)
        {
            
            var product = await _repository.GetOrderedProductByIdAsync(productId);
            if (!product.IsOrder)
            {
                 product.TotalPrices = product.TotalProduct * product.Price;
               
            
            }
           
            Console.WriteLine("--> Could not update product due to product is ordered");
            var productReadDto = _mapper.Map<OrderedProductReadDto>(product);

            return View(productReadDto);


        }


        [HttpGet]
        public async Task<IActionResult> DeleteAsync(string orderedProductId)
        {
            if (string.IsNullOrEmpty(orderedProductId))
            {
                return NotFound();
            }
            var product = await _repository.GetOrderedProductByIdAsync(orderedProductId);

            var result = await _repository.DeleteAsync(product);
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
        //
        private string GetExistingUserId()
        {
            return _userManager.GetUserId(User);
        }



    }
}