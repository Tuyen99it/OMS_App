using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using OMS_App.Data;
using OMS_App.Models;
using Microsoft.EntityFrameworkCore;
[Route("/posts/")]
public class ViewPostController : Controller
{
    private readonly ILogger<ViewPostController> _logger;
    private readonly OMSDBContext _context;
    private IMemoryCache _cache;
    // Số bài viết hiển thị trên một trang danh mục
    public const int ITEM_PER_PAGE = 4;
    public ViewPostController(ILogger<ViewPostController> logger, OMSDBContext context, IMemoryCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }

    [Route("{slug?}", Name = "listpost")]
    public async Task<IActionResult> Index([Bind(Prefix = "page")] int pageNumber, [FromRoute(Name = "slug")] string slugCategory)
    {
     
        
        var Categories = GetCategories();
        Category Category=null;
        
        if (!string.IsNullOrEmpty(slugCategory))
        {

             Category = FindCategoryBySlug(Categories, slugCategory);
            if (Category == null)
            {
                return NotFound("Không thấy Category");
            }

        }
        Console.WriteLine(Categories[0].Title);
        ViewBag.Categories=Categories;
        ViewBag.Category=Category;
        ViewBag.slugCategory=slugCategory;
       
        return View();
    }
    // lấy thông tin category có sử dụng cache
    [NonAction]
    private List<Category> GetCategories()
    {
        List<Category> categories;
        string keyCacheCategories = "_listallcategories";
        //phuc hoi cache tu Memory cache, khong co thi truy van database
        if (!_cache.TryGetValue(keyCacheCategories, out categories))
        {
            categories = _context.Categories.Include(c => c.ChildCategories).AsEnumerable().Where(c => c.ParentCategoryId == null).ToList();
            // Thiet lap cache, luu vao cache
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(300));
            _cache.Set(keyCacheCategories, categories, cacheEntryOptions);
        }

        return categories;
    }

    // Tìm đệ quy trong cây, một category theo slug
    [NonAction]
    private Category FindCategoryBySlug(List<Category> categories, string Slug)
    {
        foreach (var category in categories)
        {
            if (category.Slug == Slug) return category;
            var c1 = FindCategoryBySlug(category.ChildCategories.ToList(), Slug);
            if (c1 != null)
            {
                return c1;
            }
        }
        return null;

    }
}
