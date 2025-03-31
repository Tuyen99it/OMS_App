﻿using System.Threading.Tasks;
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
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound("Can not found category");
            }
            var post = _context.Posts
                     .Where(p => p.Id == id)
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
                    PostId=model.Post.Id,
                    CategoryId=categoryId
                });
            await _context.SaveChangesAsync();            }
            return RedirectToAction("Index", "Post");

        }

        // // GET: PostController/Edit/5
        // [HttpGet]
        // public ActionResult Edit(int? id)
        // {
        //     var model = new CategoryEditModel();
        //     if (id == null)
        //     {
        //         return NotFound("ID is null");
        //     }
        //     model.Category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
        //     model.Options = _context.Categories.Select(c => new SelectListItem()
        //     {
        //         Text = c.Title,
        //         Value = c.ParentCategoryId.ToString()
        //     }).ToList();
        //     if (model.Category == null)
        //     {
        //         return NotFound("Unable to load category");
        //     }
        //     return View(model);
        // }

        // // POST: PostController/Edit/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<ActionResult> Edit(CategoryEditModel model)
        // {
        //     _logger.LogInformation("Come to update1");
        //     if (ModelState.IsValid)
        //     {
        //         return View();
        //     }
        //     _logger.LogInformation("Come to update2");


        //     if (model.Category.ParentCategoryId == -1)
        //     {
        //         model.Category.ParentCategoryId = null;
        //     }
        //     _context.Categories.Update(model.Category);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction("Index");
        // }



        // // GET: PostController/Delete/5
        // public ActionResult Delete(int id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound("Id is null");
        //     }
        //     var category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
        //     return View(category);
        // }

        // // POST: PostController/Delete/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<ActionResult> Delete(Category category)
        // {
        //     _context.Categories.Remove(category);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction("Index");
        // }
        // private async Task<List<SelectListItem>> RenderOptions(Category category, int level)
        // {
        //     var options = new List<SelectListItem>();
        //     string prefix = String.Concat(Enumerable.Repeat("-", level));
        //     var option = new SelectListItem()
        //     {
        //         Text = prefix + category.Title,
        //         Value = category.Id.ToString()
        //     };
        //     options.Add(option);
        //     if (category.ChildCategories?.Count > 0)
        //     {
        //         foreach (var childCategory in category.ChildCategories)
        //         {
        //             var clist = await RenderOptions(childCategory, level + 1);
        //             options.AddRange(clist);
        //         }
        //     }
        //     return options;

        // }
        // public async Task<IActionResult> UploadImages()
        // {
        //     return Content("Load file successfully");
        // }
        // // hander category image
        // // [HttpPost]
        // // public async Task<IActionResult> UploadImages(string? FilePath, IFormFile? forms)
        // // {
        // //     Console.WriteLine("Come to upload file");
        // //     // if (FilePath == null || forms == null)
        // //     // {
        // //     //     return Content("Unable to load file");
        // //     // }
        // //     // // create file root path
        // //     // var directorys = FilePath.Split('_');
        // //     // string filepath = Path.Combine(directorys);
        // //     // string filepathroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", filepath);
        // //     // if (!Directory.Exists(filepathroot))
        // //     // {
        // //     //     Directory.CreateDirectory(filepathroot);
        // //     // }
        // //     // //get file from forms

        // //     // var childfilePath = Path.Combine(filepathroot, forms.FileName);
        // //     // using (var fileStream = new FileStream(childfilePath, FileMode.Create))
        // //     // {
        // //     //     await forms.CopyToAsync(fileStream);
        // //     // }

        // //     return Content("Load file successfully");

        // //}
        // [HttpPost]
        // public async Task<IActionResult> UploadImage(IFormFile uploadedFile)
        // {
        //     Console.WriteLine("Come to update");
        //     if (uploadedFile == null || uploadedFile.Length == 0)
        //     {
        //         Console.WriteLine("No file to upload");
        //         return Json(new { success = false, message = "No file uploaded." });
        //     }

        //     try
        //     {
        //         Console.WriteLine("start to update");
        //         var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "categories");
        //         if (!Directory.Exists(uploadsFolder))
        //         {
        //             Directory.CreateDirectory(uploadsFolder);
        //             Console.WriteLine("Create the upload foler");
        //         }

        //         var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
        //         var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //         using (var fileStream = new FileStream(filePath, FileMode.Create))
        //         {
        //             await uploadedFile.CopyToAsync(fileStream);
        //         }
        //         Console.WriteLine("Uuplad success");
        //         return Json(new { success = true, filePath = $"/uploads/categories/{uniqueFileName}" });

        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error uploading image");
        //         return Json(new { success = false, message = "An error occurred while uploading the file." });
        //     }
        // }

    }
}
