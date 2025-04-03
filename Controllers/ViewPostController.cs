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
        Category Category = null;

        if (!string.IsNullOrEmpty(slugCategory))
        {

            Category = FindCategoryBySlug(Categories, slugCategory);
        }
        // Lây các bài viết theo category

        var posts = _context.Posts
        .Include(p => p.Author)
        .Include(p => p.PostCategories)
        .ThenInclude(c => c.Category)
        .ToList();
        if (Category != null)
        {
            // get all childCategories Id of category
            var childCategoryIds = Category.ChildCategories.Select(c => c.Id).ToList();
            // add categoryID in the list
            childCategoryIds.Add(Category.Id);
            // Filter all post include category and its children
            posts = posts.Where(p => p.PostCategories.Where(c => childCategoryIds.Contains(c.CategoryId)).Any()).ToList();
        }
        // Lấy tổng dòng dữ liệu
        var totalItems = posts.Count();
        // Tính số trang hiên tại 
        int totalPages = (int)Math.Ceiling((double)totalItems / ITEM_PER_PAGE);
        if (totalPages < 1) totalPages = 1;
        if (pageNumber == 0) pageNumber = 1;
        // format pageNumber
        pageNumber = (pageNumber > totalPages) ? totalPages : (pageNumber >= 1) ? pageNumber : 1;
        // Get item following page Number
        posts=posts
        .Skip(ITEM_PER_PAGE*(pageNumber-1))
        .Take(ITEM_PER_PAGE)
        .OrderByDescending(p=>p.DateUpdated)
        .ToList();
        ViewData["pageNumber"]=pageNumber;
        ViewData["totalPage"]=totalPages;
        ViewBag.Categories = Categories;
        ViewBag.Category = Category;
        ViewBag.slugCategory = slugCategory;

        return View(posts);
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
