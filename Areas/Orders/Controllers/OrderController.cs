using System;
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
        private readonly IOrderedProductRepo _orderedProductRepository;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;


        public OrderController(IOrderRepo repository, ILogger<OrderController> logger, IMapper mapper, SignInManager<AppUser> signInManager, UserManager<AppUser>userManager,IOrderedProductRepo orderedProductRepository)
        {
            _repository = repository;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _orderedProductRepository = orderedProductRepository;

        }
        [HttpGet]

        public async Task<IActionResult> Index(string? orderStatus = "Create")
        {
            var model = new OrderIndexViewModel();
            Console.WriteLine("--> Come to Index get");
            var orders = await _repository.GetOrdersByStatusAsync(orderStatus);

            if (orders == null)
            {
                Console.WriteLine("--> There is no order");
                

            }
            var ordersDto = _mapper.Map<List<OrderReadDto>>(orders);
            model.OrdersReadDto = ordersDto;
            return View(model);

           

        }


        [HttpGet]

        public async Task<IActionResult> CreateAsync(string productId)

        {
            if (string.IsNullOrEmpty(productId))
            {
                Console.WriteLine("--> ProducId is " + productId);
                return NotFound("Product Id is " + productId);

            }
            var orderedProduct = await _orderedProductRepository.GetOrderedProductByIdAsync(productId);
            if (orderedProduct == null)
            {
                Console.WriteLine("--> Could not load product");
                return NotFound("Could not load product");
            }
            var order = new Order();
            var appUserId = GetExistingUserId();
            if (appUserId == null)
            {
                Console.WriteLine("User is null");
                return View();
            }
            order.UserId = appUserId;
            
            // Crate a new order
            var result = await _repository.CreateAsync(order);
            if (!result)
            {

                Console.WriteLine("--> Could not create order");
                return NotFound("Could not create order");
            }
            // get orderId
            var newestOrder = await _repository.GetNewestOrderAsync();
            // Update OrderedProduct
            orderedProduct.OrderId = newestOrder.Id;
            orderedProduct.IsOrder = true;
            var updateOrderProduct = await _orderedProductRepository.UpdateAsync(orderedProduct);
            if (!updateOrderProduct)
            {
                Console.WriteLine("--> Could not update ordered Product");
                // Reset Order
                await ResetOrder(newestOrder);
                return View();
            }
            // Update OrderStatus Update
            var orderStatus = new OrderStatusUpdate()
            {
                Status = OrderStatus.Create,
                IsStatusUpdate = true,
                UpdateTime = DateTime.Now,
                OrderId = newestOrder.Id,
                Note = "Create a new Order"
            };
            var updateStatusResult = await _repository.CreateOrderStatusAsync(orderStatus);
            if (!updateStatusResult)
            {
                Console.WriteLine("--> Could not update ordered Product");
                // Reset 
                await ResetOrderedProduct(orderedProduct);
                await ResetOrder(newestOrder);
                return View();

            }
            Console.WriteLine("Create order successfull");
            return RedirectToAction("Index");

        }
        // Update Order
        [HttpGet]
        public async Task<IActionResult> Update(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                Console.WriteLine("--> orderId is null");
                return View();
            }
            var model = new UpdateOrderViewModel();
            var existStatus = await _repository.GetOrderStatusByOrderIdAsync(orderId);
            if (existStatus == null)
            {
                Console.WriteLine("--> Could not get existing status");
                return NotFound("--> Could not get existing status");
            }

            var statuseses = existStatus.Select(s => s.Status).ToList();
             if (statuseses == null)
            {
                Console.WriteLine("--> Could not get existing status");
                return NotFound("--> Could not get existing status");
            }
            model.ExistStatus = statuseses;
            model.OrderStatusUpdate.OrderId = Convert.ToInt32(orderId);
           
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(UpdateOrderViewModel model)
        {
            var updateOrder = new OrderStatusUpdate();
            updateOrder.OrderId = model.OrderStatusUpdate.OrderId;
             updateOrder.Status = model.OrderStatusUpdate.Status;
              updateOrder.Note = model.OrderStatusUpdate.Note;

            updateOrder.UpdateTime = DateTime.Now;
          updateOrder.IsStatusUpdate = true;
            Console.WriteLine("-->Order Id"+updateOrder.OrderId);
             var updateStatusResult = await _repository.CreateOrderStatusAsync(updateOrder);
            if (!updateStatusResult)
            {
                Console.WriteLine("--> Could not update ordered Product");
                return View();

            }
            Console.WriteLine("Create order successfull");
            return RedirectToAction("Index","Order", new {status=OrderStatus.Create}
            );
        }
        private async Task ResetOrderedProduct(OrderedProduct orderedProduct)
        {

            orderedProduct.OrderId = null;
            orderedProduct.IsOrder = false;
            var updateOrderProduct = await _orderedProductRepository.UpdateAsync(orderedProduct);
            if (!updateOrderProduct)
            {
                Console.WriteLine("--> Could not reset ordered Product");
                // Reset Order

            }
            Console.WriteLine("-->Reset ordered Product sucessfully");

        }
        private async Task ResetOrder(Order Order)
        {
            var delete = await _repository.DeleteAsync(Order);
            if (!delete) Console.WriteLine("Can not delete");
            Console.WriteLine("delete");


        }






        [HttpGet]
        public async Task<IActionResult> DetailAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return NotFound("OrderId  is null");
            }
            var order = await _repository.GetOrderByIdAsync(orderId);
            var orderReadDto = _mapper.Map<OrderReadDto>(order);

            return View(order);


        }
        // Edit productName


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
        private double GetOrderedPrices(ICollection<OrderedProduct> products)
        {
            double totalPrice = 0;
            foreach (var product in products)
            {
                totalPrice += product.TotalPrices;
            }
            return totalPrice;
        }
        private string GetExistingUserId()
        {
            return _userManager.GetUserId(User);
        }



    }
}