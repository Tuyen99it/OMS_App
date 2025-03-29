using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using OMS_App.Areas.Product.Models;
using OMS_App.Data;
using OMS_App.Models;

namespace OMS_App.Areas.Product.Controllers
{

    [Area("Product")]

    public class CategoryController : Controller
    {
        private readonly OMSDBContext _context;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(OMSDBContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }        // GET: CategoryController
        public ActionResult Index()

        {
            var model = new CategoryIndexViewModel();
            model.Categories = _context.Categories
                     .Include(c => c.ChildCategories) // Nập các categories con
                     .AsEnumerable()
                     .Where(c => c.ParentCategory == null) // Lấy các parentCategory 
                     .ToList();


            return View(model);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound("Can not found category");
            }
            var category = _context.Categories
                .Include(c => c.ChildCategories).
                FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound("Can not found category");
            }
            return View(category);
        }

        // GET: CategoryController/Create
        public async Task<ActionResult> Create()
        {
            var model = new CategoryCreateModel();
            var categories = _context
                .Categories
                .Include(c => c.ChildCategories)
                .AsEnumerable()
                .Where(c => c.ParentCategory == null)
                .ToList();
            categories.Insert(0, new Category()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var category in categories)
            {
                int level = 0;
                var clist = await RenderOptions(category, level);
                list.AddRange(clist);
            }
            model.Options = list;

            return View(model);
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryCreateModel model)
        {
            _logger.LogInformation("Create category success");
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Imput is invalid");
                return View();
            }
            if (model.ParentCategoryId == null)
            {
                _logger.LogInformation("ParentCategoryId is null");
            }
            if (model.ParentCategoryId == -1)
            {
                model.Category.ParentCategoryId = null;
            }
            else
            {
                model.Category.ParentCategoryId = model.ParentCategoryId;
            }
            _logger.LogInformation("ParentCategoryId" + model.Category.ParentCategoryId);
            _context.Categories.Add(model.Category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Create category success1");
            return RedirectToAction("Index", "Category");

        }

        // GET: CategoryController/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var model = new CategoryEditModel();
            if (id == null)
            {
                return NotFound("ID is null");
            }
            model.Category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
            model.Options = _context.Categories.Select(c => new SelectListItem()
            {
                Text = c.Title,
                Value = c.ParentCategoryId.ToString()
            }).ToList();
            if (model.Category == null)
            {
                return NotFound("Unable to load category");
            }
            return View(model);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryEditModel model)
        {
            _logger.LogInformation("Come to update1");
            if (ModelState.IsValid)
            {
                return View();
            }
            _logger.LogInformation("Come to update2");


            if (model.Category.ParentCategoryId == -1)
            {
                model.Category.ParentCategoryId = null;
            }
            _context.Categories.Update(model.Category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return NotFound("Id is null");
            }
            var category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
            return View(category);
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        private async Task<List<SelectListItem>> RenderOptions(Category category, int level)
        {
            var options = new List<SelectListItem>();
            string prefix = String.Concat(Enumerable.Repeat("-", level));
            var option = new SelectListItem()
            {
                Text = prefix + category.Title,
                Value = category.Id.ToString()
            };
            options.Add(option);
            if (category.ChildCategories?.Count > 0)
            {
                foreach (var childCategory in category.ChildCategories)
                {
                    var clist = await RenderOptions(childCategory, level + 1);
                    options.AddRange(clist);
                }
            }
            return options;

        }
        public async Task<IActionResult> UploadImages()
        {
            return Content("Load file successfully");
        }
        // hander category image
        // [HttpPost]
        // public async Task<IActionResult> UploadImages(string? FilePath, IFormFile? forms)
        // {
        //     Console.WriteLine("Come to upload file");
        //     // if (FilePath == null || forms == null)
        //     // {
        //     //     return Content("Unable to load file");
        //     // }
        //     // // create file root path
        //     // var directorys = FilePath.Split('_');
        //     // string filepath = Path.Combine(directorys);
        //     // string filepathroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", filepath);
        //     // if (!Directory.Exists(filepathroot))
        //     // {
        //     //     Directory.CreateDirectory(filepathroot);
        //     // }
        //     // //get file from forms

        //     // var childfilePath = Path.Combine(filepathroot, forms.FileName);
        //     // using (var fileStream = new FileStream(childfilePath, FileMode.Create))
        //     // {
        //     //     await forms.CopyToAsync(fileStream);
        //     // }

        //     return Content("Load file successfully");

        //}
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile uploadedFile)
        {
            Console.WriteLine("Come to update");
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                Console.WriteLine("No file to upload");
                return Json(new { success = false, message = "No file uploaded." });
            }

            try
            {
                Console.WriteLine("start to update");
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "categories");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Console.WriteLine("Create the upload foler");
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                Console.WriteLine("Uuplad success");
                return Json(new { success = true, filePath = $"/uploads/categories/{uniqueFileName}" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return Json(new { success = false, message = "An error occurred while uploading the file." });
            }
        }

    }
}
