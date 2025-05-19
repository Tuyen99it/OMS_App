using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OMS_App.Models;

namespace OMS_App.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public AdminController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Order()
    {
        return View();
    }
    public IActionResult Inventory()
    {
        return View();
    }
    public IActionResult Voucher()
    {
        return View();
    }

}
