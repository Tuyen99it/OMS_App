using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using OMS_App.Areas.Orders.Data;
using OMS_App.Areas.Orders.Dtos;
using OMS_App.Areas.Orders.Models;
namespace OMS_App.Areas.Orders.Controllers
{
    [Area("Orders")]
    public class OrderedProductController : Controller
    {
        private readonly IOrderedProductRepo _repository;
        private readonly ILogger<OrderedProductController> _logger;
        private readonly IMapper _mapper;
  
        public OrderedProductController(IOrderedProductRepo repository, ILogger<OrderedProductController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
          
        }
 
        public async Task<IActionResult> Index(string orderId)
        {
             Console.WriteLine("--> Come to Index get"); 
            var products = await _repository.GetAllOrderedProductByOrderIdAsync(orderId);

            if (products != null)
            {
                var productsReadDto = _mapper.Map<List<OrderedProductReadDto>>(products);
                return View(productsReadDto);

            }
      
            return View();
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(OrderedProductCreateDto dto)
        {
            

            Console.WriteLine("-->Start create  Order product");
           
            var product = _mapper.Map<OrderedProduct>(dto);
            var existProductName = await _repository.GetOrderedProductByNameAsync(dto.ProductName);
            if (existProductName == null)
            {
                Console.WriteLine("-->Item Name is not in the stock, need to add image");

                var result = await _repository.CreateAsync(product);
                if (result == true)
                {  
                    return View();
                }
                else
                {
                    Console.WriteLine("-->Can not add product");
                    return View(dto);
                }

            }

            else
            {
                Console.WriteLine("--> Product Name is existing");
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> DetailAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return NotFound("ProductId is null");
            }
            var product = await _repository.GetOrderedProductByIdAsync(productId);
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



    }
}