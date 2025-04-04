using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using OMS_App.Data;
using OMS_App.Models;
using Microsoft.EntityFrameworkCore;
[Route("/product/")]
public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly OMSDBContext _context;
    private IMemoryCache _cache;
    // Số bài viết hiển thị trên một trang danh mục
    public const int ITEM_PER_PAGE = 4;
    public ProductController(ILogger<ProductController> logger, OMSDBContext context, IMemoryCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }


    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);

    }
}
