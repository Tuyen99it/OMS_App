using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OMS_App.Models;

namespace OMS_App.Controllers;

public class InventoryController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public InventoryController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Report()
    {
        return View();
    }
    public IActionResult Create()
    {
        return View();
    }
    public IActionResult Update()
    {
        return View();
    }
    public IActionResult Delete()
    {
        return View();
    }

}
