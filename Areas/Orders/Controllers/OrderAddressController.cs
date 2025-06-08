using System.Linq.Expressions;
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
    public class OrderAddressController : Controller
    {
        private readonly IOrderAddressRepo _repository;
        private readonly ILogger<OrderAddressController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public OrderAddressController(IOrderAddressRepo repository, ILogger<OrderAddressController> logger, IMapper mapper, UserManager<AppUser> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index(string userId)
        {
            Console.WriteLine("--> Come to Index get");

            var currentUserId = _userManager.GetUserId(User);
            var addresses = await _repository.GetAllOrderAddressesByUserIdAsync(currentUserId);

            if (addresses != null)
            {
                var addressesReadDto = _mapper.Map<List<OrderAddress>>(addresses);
                return View(addressesReadDto);

            }

            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(OrderAddressCreateDto dto)
        {

            Console.WriteLine("-->Start create order");

            var address = _mapper.Map<OrderAddress>(dto);


            var result = await _repository.CreateAsync(address);
            if (result == true)
            {
                return View("Index", new { sortUpdate = "desceding" });
            }
            else
            {
                Console.WriteLine("-->Can not add orders");
                return View(address);
            }

        }



        [HttpGet]
        public async Task<IActionResult> DetailAsync(string addressId)
        {
            if (string.IsNullOrEmpty(addressId))
            {
                return NotFound("AddressId is null");
            }
            var address = await _repository.GetOrderAddressByIdAsync(addressId);
            var addressReadDto = _mapper.Map<OrderReadDto>(address);

            return View(addressReadDto);


        }
        // Edit productName

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(OrderAddressUpdateDto dto)
        {

            var existOrder = await _repository.GetOrderAddressByIdAsync(dto.Id.ToString());
            if (existOrder == null)
            {
                Console.WriteLine("--> Address Id is not existing");
                return View();

            }
            var address = _mapper.Map<OrderAddress>(dto);
            var result = await _repository.UpdateAsync(address);
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
        public async Task<IActionResult> DeleteAsync(string addressId)
        {
            if (string.IsNullOrEmpty(addressId))
            {
                return NotFound();
            }
            var address = await _repository.GetOrderAddressByIdAsync(addressId);

            var result = await _repository.DeleteAsync(address);
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