using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS_App.Models;
using OMS_App.Areas.Admin.UserRole.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using OMS_App.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace OMS_App.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class UserRoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly OMSDBContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleController> _logger;
        public UserRoleController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, OMSDBContext context, ILogger<RoleController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }
        [TempData]
        public string StatusMessage { get; set; }



        // GET: UserList
        [HttpGet]
        public async Task<ActionResult> UserList()
        {
            var model = new UserListModel();

            var users = await _userManager.Users.ToListAsync();
            model.Users = users.Select(u => new UserAndRole()
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            }).ToList();
            foreach (var user in model.Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Any())
                {
                    user.RoleNames = "No role";
                }
                else
                {
                    user.RoleNames = string.Join(",", roles.ToList());
                }

            }
            return View(model);
        }
        // Post: UserList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserList(UserListModel model)
        {

            if (string.IsNullOrEmpty(model.SearchUserName))
            {
                var users = await _userManager.Users.ToListAsync();
                model.Users = users.Select(u => new UserAndRole()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToList();
                foreach (var user in model.Users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (!roles.Any())
                    {
                        user.RoleNames = "No role";
                    }
                    else
                    {
                        user.RoleNames = string.Join(",", roles.ToList());
                    }

                }
            }
            else
            {
                var users = _userManager.Users.Where(u => u.UserName.Contains(model.SearchUserName));
                model.Users = users.Select(u => new UserAndRole()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToList();
                foreach (var user in model.Users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (!roles.Any())
                    {
                        user.RoleNames = "No role";
                    }
                    else
                    {
                        user.RoleNames = string.Join(",", roles.ToList());
                    }

                }
            }
            return View(model);


        }



        [HttpGet]
        public async Task<IActionResult> AddRole(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("user id is null");
                return NotFound("User id is null");
            }
            var model = new AddRoleViewModel();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("Unable to load user");
                return NotFound("Unable to load user");
            }
            var existingRoles = (await _userManager.GetRolesAsync(user)).ToList();
            var totalRoles = await _roleManager.Roles.ToListAsync();
            var restOfRoles = totalRoles.Where(r => !existingRoles.Contains(r.Name)).ToList();
            model.User = user;
            model.ExistingRoles = existingRoles;
            
            model.Options = restOfRoles.Select(r => new SelectListItem()
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();
            model.Options.Insert(0,new SelectListItem(){
                Text="The list of avalable role",
                Value="0"
            });

            return View(model);

        }

        
        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleViewModel model, string userId=null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("user id is null");
                return NotFound("User id is null");
            }
            if(string.IsNullOrEmpty(model.SelectedRole)){
                _logger.LogError("Selected role is null");
                return NotFound("Selected role id is null");
            }
           
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("Unable to load user");
                return NotFound("Unable to load user");
            }
            var addUserRole=await _userManager.AddToRoleAsync(user,model.SelectedRole);
            if (!addUserRole.Succeeded){
                 _logger.LogError("Unable to add role for user");
                 StatusMessage="Unable to add role for user";
                return View();
                

            }
             _logger.LogInformation("Add role for user successfull");
             return RedirectToAction("AddRole",new {userId=user.Id});

            


        }

         [HttpGet]
        public async Task<IActionResult> DeleteAsync( string userId=null, string roleName=null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("user id is null");
                return NotFound("User id is null");
            }
            if(string.IsNullOrEmpty(roleName)){
                _logger.LogError("Selected role is null");
                return NotFound("Selected role id is null");
            }
           
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                _logger.LogError("Unable to load user ");
                return NotFound("Unable to load user ");
            }
            var removeRole=await _userManager.RemoveFromRoleAsync(user,roleName);
            if (!removeRole.Succeeded){
                 _logger.LogError("Unable to remove role for user");
                 StatusMessage="Unable to remove role for user";
                return RedirectToAction("AddRole",new {userId=user.Id});
                

            }
             _logger.LogInformation("Remove role for user successfull");
             return RedirectToAction("AddRole",new {userId=user.Id});

            

        }
    }
}
