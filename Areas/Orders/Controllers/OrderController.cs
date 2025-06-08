using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using OMS_App.Areas.Orders.Data;
using OMS_App.Areas.Orders.Dtos;
using OMS_App.Areas.Orders.Models;
using OMS_App.Models;
namespace OMS_App.Areas.Orders.Controllers
{
    [Area("Orders")]
    public class OrderController : Controller
    {
        private readonly IOrderRepo _repository;
        private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;


        public OrderController(IOrderRepo repository, ILogger<OrderController> logger, IMapper mapper, SignInManager<AppUser> signInManager)
        {
            _repository = repository;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;

        }
        [HttpGet]

        public async Task<IActionResult> Index(string? orderStatus = "Create")
        {
            var model = new OrderIndexViewModel();
            Console.WriteLine("--> Come to Index get");
            var orders = await _repository.GetOrdersByStatusAsync(orderStatus);

            if (orders != null)
            {
                var ordersDto = _mapper.Map<List<OrderReadDto>>(orders);
                model.OrdersReadDto = ordersDto;
                return View(model);

            }

            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(OrderCreateDto dto)
        {

            Console.WriteLine("-->Start create order");
            // if (!ModelState.IsValid)
            // {
            //     Console.WriteLine("-->Input invalid");
            //     return View();
            // }

            var order = _mapper.Map<Order>(dto);


            var result = await _repository.CreateAsync(order);
            if (result == true)
            {
                return View("Index", new { sortUpdate = "desceding" });
            }
            else
            {
                Console.WriteLine("-->Can not add orders");
                return View(order);
            }

        }



        [HttpGet]
        public async Task<IActionResult> DetailAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return NotFound("ProductId is null");
            }
            var order = await _repository.GetOrderByIdAsync(orderId);
            var orderReadDto = _mapper.Map<OrderReadDto>(order);

            return View(order);


        }
        // Edit productName

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(OrderUpdateDto dto, string orderId)
        {

            var existOrder = await _repository.GetOrderByIdAsync(orderId);
            if (existOrder == null)
            {
                Console.WriteLine("--> Category Id is not existing");
                return View();

            }
            var order = _mapper.Map<Order>(dto);
            var result = await _repository.UpdateAsync(order);
            if (result)
            {
                return RedirectToAction("Index", new { softbyDate = true });
            }
            else
            {
                Console.WriteLine("--> Could not update Order Status");
                return View();

            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return NotFound();
            }
            var order = await _repository.GetOrderByIdAsync(orderId);

            var result = await _repository.DeleteAsync(order);
            if (result)
            {
                return RedirectToAction("Index", new { softbyDate = true });
            }
            else
            {
                Console.WriteLine("--> Could not delete  orderId");
                return View();

            }
        }



    }
}