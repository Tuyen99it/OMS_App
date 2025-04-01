using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using OMS_App.Areas.Post.Models;
using OMS_App.Data;
using OMS_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace OMS_App.Areas.Post.Controllers
{

    [Area("Post")]

    public class PostController : Controller
    {
        private readonly OMSDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PostController> _logger;
        public PostController(OMSDBContext context, ILogger<PostController> logger, UserManager<AppUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }        // GET: PostController
        public ActionResult Index()
        {

            var posts = _context.Posts
                     .Include(p => p.Author) // get author
                      .Include(p => p.PostCategories) // Nập các categories con
                      .ThenInclude(p => p.Category)
                      .OrderByDescending(p => p.DateCreated)
                      .ToList();
            return View(posts);
        }

        // GET: PostController/Details/5
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound("Can not found category");
            }
            var post = _context.Posts
                     .Where(p => p.PostId == id)
                     .Include(p => p.Author)
                     .Include(p => p.PostCategories)
                     .ThenInclude(p => p.Category)
                     .FirstOrDefault();

            if (post == null)
            {
                return NotFound("Can not found category");
            }
            return View(post);
        }

        // GET: PostController/Create
        // Lấy thông tin về author theo tài khoản đăng nhập, lấy list category
        // Sử dụng multi select trong Razor
        //1. Tạo một mảng lưu trữ các categortIds

        public async Task<ActionResult> Create()
        {
            var model = new PostCreateViewModel();

            model.Author = await _userManager.GetUserAsync(User);
            if (model.Author == null)
            {
                _logger.LogInformation("Unable to load user");
                return NotFound("Unable to load user");
            }
            // Lấy thông tin category
            var categories = _context.Categories.ToList();

            var options = new MultiSelectList(categories, "Id", "Title");
            model.Options = options;

            return View(model);
        }

       // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PostCreateViewModel model)
        {
            _logger.LogInformation("Create category success");
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Imput is invalid");
                return View();
            }
            if (model.CategoriesId == null)
            {
                _logger.LogInformation("CategoriesId are null");
            }
            
           model.Post.AuthorId=(await _userManager.GetUserAsync(User)).Id;
           model.Post.DateCreated=DateTime.Now;
            _context.Posts.Add(model.Post);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Create Post success1");
            
            // Thêm thông tin về PostCategoryId của bài viết
            foreach (var categoryId in model.CategoriesId){
                _context.PostCategories.Add(new PostCategory(){
                    PostId=model.Post.PostId,
                    CategoryId=categoryId
                });
            await _context.SaveChangesAsync();            }
            return RedirectToAction("Index", "Post");

        }

        public async Task<ActionResult> Edit(int? id)
        {
            var model = new PostEditViewModel();

            var author = await _userManager.GetUserAsync(User);
            if (author == null)
            {
                _logger.LogInformation("Unable to load user");
                return NotFound("Unable to load user");
            }

            // Lấy thông tin Post
           var post=_context.Posts.Where(p=>p.PostId==id).Include(p=>p.Author).Include(p=>p.PostCategories).ThenInclude(p=>p.Category).FirstOrDefault();
            if(author!=post.Author){
                return NotFound("Author is different, can not update");
            }
            var categories= await _context.Categories.ToListAsync();
            model.Post=post;
            model.Author=author;
            model.Options= new MultiSelectList(categories,"Id","Title");
            
            return View(model);
        }

       // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PostEditViewModel model)
        {
            _logger.LogInformation("Create category success");
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Imput is invalid");
                return View();
            }
            if (model.CategoriesId == null)
            {
                _logger.LogInformation("CategoriesId are null");
            }
           
           
            
           model.Post.AuthorId=(await _userManager.GetUserAsync(User)).Id;
           model.Post.DateUpdated=DateTime.Now;
            _context.Posts.Update(model.Post);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Create Post success1");
            
            // Thêm thông tin về PostCategoryId của bài viết
            foreach (var categoryId in model.CategoriesId){
                _context.PostCategories.Update(new PostCategory(){
                    PostId=model.Post.PostId,
                    CategoryId=categoryId
                });
            await _context.SaveChangesAsync();            }
            return RedirectToAction("Index", "Post");

        }


        // GET: PostController/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return NotFound("Id is null");
            }
            var post = _context.Posts.Where(c => c.PostId == id).FirstOrDefault();
            return View(post);
        }

        // POST: PostController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(OMS_App.Areas.Post.Models.Post post)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Post");
        }
       

        

    }
}
