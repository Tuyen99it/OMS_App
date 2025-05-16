using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using OMS_App.Data;
using OMS_App.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace OMS_App.Controllers;
[Route("/product/")]
public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly OMSDBContext _context;
    private IMemoryCache _cache;
    public const string CARTKEY = "cart";
    // Số bài viết hiển thị trên một trang danh mục
    public const int ITEM_PER_PAGE = 4;
    public ProductController(ILogger<ProductController> logger, OMSDBContext context, IMemoryCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    // Hien thi cac item product
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);

    }


    // Get CartItem from session
    public List<CartItem> GetCartITems()
    {
        // Lấy đối tượng ISession
        var session = HttpContext.Session;
        // lấy chuỗi Json lưu trong cart
        var cartJson = session.GetString(CARTKEY);

        if (cartJson != null)
        {
            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }
        return new List<CartItem>();
    }

    // Xoa Cart khoi session
    public void DeleteCartSession()
    {
        var session = HttpContext.Session;
        session.Remove(CARTKEY);
    }

    // Thêm danh sách CartITem vào session
    public void SaveCartSession(List<CartItem> cartItems)
    {
        var session = HttpContext.Session;
        var cartItemJson = JsonConvert.SerializeObject(cartItems);
        session.SetString(CARTKEY, cartItemJson);
    }

  
    // them san pham vao cart
    [Route("/addcart/{productId:int}")]
    [HttpGet]
    public IActionResult AddToCart(int productId)
    {
        if (productId == null)
        {
            return NotFound("ProductId can not equal null");
        }
        var product = _context.Products.Where(p => p.ProductId == productId).FirstOrDefault();
        if (product == null) return NotFound();
        // xu ly dua vao cart
        var cart = GetCartITems();
        var cartitem = cart.Find(p => p.Product.ProductId == productId);
        if (cartitem != null)
        {
            // Đã tồn tại, tăng thêm 1
            cartitem.Quantity++;
        }
        else
        {
            //  Thêm mới
            cart.Add(new CartItem() { Quantity = 1, Product = product });
        }

        // Lưu cart vào Session
        SaveCartSession(cart);
        return RedirectToAction(nameof(Cart));
}
// Xoa Item trong cart

[Route("/deletecart/{productId:int}")]
[HttpGet]
public IActionResult DeleteFromCart(int productId)
{

     var cart = GetCartITems ();
    var cartitem = cart.Find (p => p.Product.ProductId == productId);
    if (cartitem != null) {
        // Đã tồn tại, tăng thêm 1
        cart.Remove(cartitem);
    }

    SaveCartSession (cart);
    return RedirectToAction (nameof (Cart));
}

/// Cập nhật
[Route("/updatecart", Name = "updatecart")]
[HttpPost]
public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
{
     // Cập nhật Cart thay đổi số lượng quantity ...
    var cart = GetCartITems ();
    var cartitem = cart.Find (p => p.Product.ProductId == productid);
    if (cartitem != null) {
        // Đã tồn tại, tăng thêm 1
        cartitem.Quantity = quantity;
    }
    SaveCartSession (cart);
    // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
    return Ok();
}
[Route("/checkout")]
public IActionResult CheckOut()
{
    // Xử lý khi đặt hàng
    return View();
}

// Hien thi gio hang
[Route("/cart")]
public IActionResult Cart()
{
    return View(GetCartITems());
}

}
