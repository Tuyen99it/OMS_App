using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS_App.Areas.Admin.Models;

namespace OMS_Webapp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleController> _logger;
        public RoleController(RoleManager<IdentityRole> roleManager, ILogger<RoleController> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }
        [TempData]
        public string StatusMessage { get; set; }



        // GET: RoleController
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            _logger.LogInformation("Already enter index");
            var roles = new List<IdentityRole>();
            roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RoleManagement model)
        {
            var Roles = new List<IdentityRole>();
            if (!string.IsNullOrEmpty(model.SearchingRoleName))
            {
                var roles = await _roleManager.Roles.ToListAsync();
                Roles = roles.Where(role => role.Name.Contains(model.SearchingRoleName)).ToList();
            }
            else
            {
                Roles = await _roleManager.Roles.ToListAsync();
            }
            ViewBag.Roles = Roles;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            // Create Role Id
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("NG validate input");
            }
            if (string.IsNullOrEmpty(model.RoleName))
            {
                _logger.LogError("RoleName is null");
                return View();
            }
            //Create a new role
            var Role = new IdentityRole()
            {
                Name = model.RoleName
            };
            // Save role
            var result = await _roleManager.CreateAsync(Role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role is created successfully");
                return RedirectToAction("Index", "Role");
            }
            StatusMessage = "Error";
            foreach (var er in result.Errors)
            {

                StatusMessage += er.Description;
                _logger.LogError("Can not create Role" + StatusMessage);

            }
            return View();

        }

         [HttpGet]
        public async Task<ActionResult> Detail( string roleId=null)
        {
            if(string.IsNullOrEmpty(roleId)){
                _logger.LogError("Role Id is null");
                return View();
            }
           var role=await _roleManager.FindByIdAsync(roleId);
           if(role!=null){
            _logger.LogInformation("Load role successfully");
            return View(role);
           }
           return NotFound("Unable to load role");
        }

         [HttpGet]
        public async Task<ActionResult> Update(  string roleId=null)
        {
            if(string.IsNullOrEmpty(roleId)){
                _logger.LogError("Role Id is null");
                return View();
            }
           var role=await _roleManager.FindByIdAsync(roleId);
           if(role==null){
            _logger.LogError("Unable to load role");
             return NotFound("Unable to load role");
           }
          
           return View (role);
           
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateAsync(UpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("NG validate input");
            }
            if (string.IsNullOrEmpty(model.RoleName))
            {
                _logger.LogError("RoleName is null");
                return View();
            }
            //Create a new role
            var Role = new IdentityRole()
            {
                Name = model.RoleName
            };
            // updat role
            var result = await _roleManager.UpdateAsync(Role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role is updated successfully");
                return RedirectToAction("Index", "Role");
            }
            StatusMessage = "Error";
            foreach (var er in result.Errors)
            {

                StatusMessage += er.Description;
                _logger.LogError("Can not update Role" + StatusMessage);

            }
            return View();

        }

          [HttpGet]
        public async Task<ActionResult> Delete(  string roleId=null)
        {
            if(string.IsNullOrEmpty(roleId)){
                _logger.LogError("Role Id is null");
                return View();
            }
           var role=await _roleManager.FindByIdAsync(roleId);
           if(role==null){
            _logger.LogError("Unable to load role");
             return NotFound("Unable to load role");
           }
          
          var result=await _roleManager.DeleteAsync(role);
          if(result.Succeeded){
             _logger.LogInformation("Delete {role} success"+role.Name);
             return RedirectToAction("Index","Role");
          }
           foreach (var er in result.Errors)
            {

                StatusMessage += er.Description;
                _logger.LogError("Can not update Role" + StatusMessage);

            }
            return View();
          
           
        }


       



    }
}
