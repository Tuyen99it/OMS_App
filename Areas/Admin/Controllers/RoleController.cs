using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS_App.Models;
using OMS_App.Areas.Admin.Role.Models;
using System.Threading.Tasks;
namespace OMS_App.Areas.Admin.Controllers
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
            var roles = await _roleManager.Roles.ToListAsync();

            ViewBag.Roles = roles;

            return View();
        }
        // Post: RoleController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IndexAsync(IndexViewModel model)
        {
            if (string.IsNullOrEmpty(model.SearchingRoleName))
            {
                var roles = await _roleManager.Roles.ToListAsync();
                ViewBag.Roles = roles;
                return View();
            }
            else
            {
                var roles = await _roleManager.Roles.ToListAsync();
                roles = roles.Where(role => role.NormalizedName.Contains(model.SearchingRoleName.ToUpper())).ToList();
                ViewBag.Roles = roles;
                return View();
            }


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
            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role != null)
            {
                StatusMessage = "Role already existing";
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
        public async Task<ActionResult> Detail(string roleId = null)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                _logger.LogError("Role Id is null");
                return View();
            }
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                _logger.LogInformation("Load role successfully");
                return View(role);
            }
            return NotFound("Unable to load role");
        }

        [HttpGet]
        public async Task<ActionResult> Update(string roleId = null)
        {
            _logger.LogInformation("Access to Update");
            if (string.IsNullOrEmpty(roleId))
            {
                _logger.LogError("Role Id is null");
                return View();
            }
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogError("Unable to load role");
                return NotFound("Unable to load role");
            }
            _logger.LogInformation("Load {roleName} sucessful", role.Name);

            return View(role);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateAsync(IdentityRole model, string roleId = null)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("NG validate input");
            }
            if (string.IsNullOrEmpty(roleId))
            {
                _logger.LogError("RoleName is null");
                return View();
            }
            //find current role
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogError("Unable to load role");
                return NotFound("Unable to load role");
            }
            role.Name = model.Name;
            // updat role
            var result = await _roleManager.UpdateAsync(role);
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
            return View("Index");

        }
        public IActionResult Delete(string roleId = null)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(string roleId = null)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                _logger.LogError("Role Id is null");
                return View();
            }
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogError("Unable to load role");
                return NotFound("Unable to load role");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = $"Delete {role.Name} successfully";
                _logger.LogInformation("Delete {role} successfully" + role.Name);
                return RedirectToAction("Index", "Role");
            }
            foreach (var er in result.Errors)
            {

                StatusMessage += er.Description;
                _logger.LogError("Can not update Role" + StatusMessage);

            }
            return View();


        }

        [HttpGet]
        public async Task<IActionResult> ClaimHome(string roleId = null)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return NotFound("Unable to load role Id");
            }
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogError("unable to load roleid: " + roleId);
            }
            var claims = await _roleManager.GetClaimsAsync(role);
            ViewBag.Claims = claims;
            ViewBag.Role = role;
            return View();

        }

        // Post: RoleController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClaimHome(ClaimViewModel model,string roleId=null)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return NotFound("Unable to load role Id");
            }
            if (string.IsNullOrEmpty(model.SearchingClaimName))
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    _logger.LogError("unable to load roleid: " + roleId);
                }
                var claims = await _roleManager.GetClaimsAsync(role);
                ViewBag.Claims = claims;
                ViewBag.Role = role;
                return View();
            }
            else
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    _logger.LogError("unable to load roleid: " + roleId);
                }
                var claims = await _roleManager.GetClaimsAsync(role);
                claims= claims.Where(claim=>claim.Type.Contains(model.SearchingClaimName)).ToList();
                ViewBag.Claims = claims;
                ViewBag.Role = role;
                return View();
            }


        }

        
        [HttpGet]
        public async Task<ActionResult> CreateClaim(string roleId=null)
        {
            if(string.IsNullOrEmpty(roleId)){
                return NotFound("RoleId is null");
            }
            var role=await _roleManager.FindByIdAsync(roleId);
            ViewBag.Role=role; 
            // Create Role Id
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateClaimAsync(ClaimCreateViewModel model,string roleId=null)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("NG validate input");
            }
            if (string.IsNullOrEmpty(model.ClaimType))
            {
                _logger.LogError("ClaimType is null");
                return View();
            }
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
               return NotFound("Unable to load role");
            }
            var claim = new Claim(model.ClaimType, model.ClaimValue);
            // Save role
            var result = await _roleManager.AddClaimAsync(role,claim);
            if (result.Succeeded)
            {
                _logger.LogInformation("Claim is created successfully");
                return RedirectToAction("ClaimHome", "Role",new {roleId=role.Id});
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
       
        public async Task<ActionResult> DeleteClaimAsync(string claimtype=null,string claimvalue=null, string roleId = null)
        {
            if(string.IsNullOrEmpty(roleId)||string.IsNullOrEmpty(claimtype)||string.IsNullOrEmpty(claimvalue)){
                return NotFound("Can not delete due to role or claim is null");
            }
           
            var role=await _roleManager.FindByIdAsync(roleId);
        
            var removeClaim=new Claim(claimtype,claimvalue);
            var  result=await _roleManager.RemoveClaimAsync(role,removeClaim);
            if(result.Succeeded){
                _logger.LogInformation("Remove Claim successfully");
                return RedirectToAction("ClaimHome",new{roleId=role.Id});
            }
            foreach(var err in result.Errors){
                StatusMessage+=err.Description.ToString();
            }
            _logger.LogError("Can not delete role");
            return RedirectToAction("ClaimHome",new{roleid=role.Id});
           

        }









    }
}
