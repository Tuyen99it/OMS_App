using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
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
            if(id == null)
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
            var model=new CategoryCreateModel();
            var categories = _context
                .Categories
                .Include (c => c.ChildCategories)
                .AsEnumerable()
                .Where(c => c.ParentCategory == null)
                .ToList();
            categories.Insert(0, new Category()
            {
                Id = -1,
                Title="Không có danh mục cha"
            });
            List<SelectListItem> list = new List<SelectListItem>();
            foreach( var category in categories)
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
        public async Task< ActionResult> Create(CategoryCreateModel model)
        {
            _logger.LogInformation("Create category success");
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Imput is invalid");
                return View();
            }
            if (model.ParentCategoryId == null)
            {
                _logger.LogInformation("ParentCategoryId is null" );
            }
            if (model.ParentCategoryId == -1)
            {
                model.Category.ParentCategoryId = null;
            }
            else
            {
                model.Category.ParentCategoryId = model.ParentCategoryId;
            }
            _logger.LogInformation("ParentCategoryId"+model.Category.ParentCategoryId);
            _context.Categories.Add(model.Category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Create category success1");
            return RedirectToAction("Index","Category");
           
        }

        // GET: CategoryController/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var model =new CategoryEditModel();
            if (id == null)
            {
                return NotFound("ID is null");
            }
            model.Category= _context.Categories.Where(c => c.Id == id).FirstOrDefault();
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
            var category=_context.Categories.Where(c=>c.Id == id).FirstOrDefault();
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
        private async Task<List<SelectListItem>> RenderOptions(Category category,int level)
        {
            var options=new List<SelectListItem>();
            string prefix = String.Concat(Enumerable.Repeat("-", level));
            var option = new SelectListItem()
            {
                Text = prefix + category.Title,
                Value = category.Id.ToString()
            };
            options.Add(option);
            if (category.ChildCategories?.Count > 0) {
                foreach (var childCategory in category.ChildCategories)
                {
                   var clist= await RenderOptions(childCategory, level + 1);
                    options.AddRange(clist);
                }
            }
            return options;

        }
       
    }
}
