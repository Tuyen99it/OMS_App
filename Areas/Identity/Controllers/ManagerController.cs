using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using OMS_App.Areas.Identity.Models;
using OMS_Webapp.Models;

namespace OMS_App.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("/Manager/[action]")]
    public class ManagerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserStore<AppUser> _userStore;

        private readonly ILogger<ManagerController> _logger;
        private readonly IEmailSender _emailSender;
        public ManagerController(ILogger<ManagerController> logger,UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [TempData]
        public string ErrorMessage { get; set; }

        [TempData]
        public string StatusMessasge { get; set; }
        [HttpGet("/index/")]
        public async Task<IActionResult> Index()
        {
            var user=await _userManager.GetUserAsync(User);
            if(user==null)
            {
                return RedirectToAction("Login","Account");
            }
            var model=new ProfileViewModel
            {
                PhoneNumber=user.PhoneNumber
            };
            return View(model);

        }
        
        [HttpPost("/index/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model){
            var user =await _userManager.GetUserAsync(User);
            if(user==null){
                return RedirectToAction("Login","Acccount");    
            }
            if(!ModelState.IsValid){
                ErrorMessage="Số điện thoại không hợp lệ, vui lòng thử lại";
                return View(model);
            }
            user.PhoneNumber=model.PhoneNumber;
            var result=await _userManager.UpdateAsync(user);
            if(result.Succeeded){
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors){
                ModelState.AddModelError(string.Empty,error.Description);
            }
            return View(model);
        }

        [HttpGet("/email/")]
        public async Task<IActionResult> Email()
        {
            var user=await _userManager.GetUserAsync(User);
            if(user==null)
            {
                return RedirectToAction("Login","Account");
            }
            var model=new EmailViewModel
            {
                Email=user.Email
            };
            return View(model);

        }

        [HttpPost("/email/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Email(EmailViewModel model)
        {
            var user =await _userManager.GetUserAsync(User);
            if(user==null){
                return RedirectToAction("Login","Acccount");    
            }
            if(!ModelState.IsValid){
                ErrorMessage="Địa chỉ email không hợp lệ, vui lòng thử lại";
                return View(model);
            }
            if(user.Email!=model.Email)
            {
                var code=await _userManager.GenerateChangeEmailTokenAsync(user,model.Email);
                code=WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));
                    var  callbackUrl=Url.Action(
                    "ConfirmEmail",
                    controller:"Account",
                    new { area="Identity",userId=user.Id,code=code},
                    protocol:HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email,"Xác nhận email",
                    $"Vui lòng xác nhận email của bạn bằng cách <a href='{System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callbackUrl)}'>click vào đây</a>.");
                ErrorMessage="Link xác nhận đã được gửi đến email của bạn";
                return View(model);

            }
            ErrorMessage="Email không thay đổi";
            return View(model);
        }

        [HttpGet("/changepassword/")]
        public async Task<IActionResult> ChangePassword()
        {
            var user=await _userManager.GetUserAsync(User);
            if(user==null)
            {
                return RedirectToAction("Login","Account");
            }
            
            return View();
        }

        [HttpPost("/changepassword/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePaswordViewModel model)
        {
            var user =await _userManager.GetUserAsync(User);
            if(user==null){
                return RedirectToAction("Login","Acccount");    
            }
            if(!ModelState.IsValid){
                ErrorMessage="Mật khẩu không hợp lệ, vui lòng thử lại";
                return View(model);
            }
           var result=await _userManager.ChangePasswordAsync(user,model.OldPassword,model.NewPassword);
           if(result.Succeeded){
                await _signInManager.RefreshSignInAsync(user);
                ErrorMessage="Mật khẩu đã được thay đổi";
                return RedirectToAction("ChangePassword");
                  }
            foreach (var error in result.Errors){
                ModelState.AddModelError(string.Empty,error.Description); 
            }
             return View(model);
        }

        [HttpGet("/externallogin/")]
        public async Task<IActionResult>ExternalLogin(){
           var user = await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+ _userManager.GetUserId(User));
            }
            var currentLogin=await _userManager.GetLoginsAsync(user); // get current login provider
            var otherLogins=(await _signInManager.GetExternalAuthenticationSchemesAsync()) // get all external auth schemes was configured in the service.
                .Where(auth=>currentLogin.All(ul=>auth.Name!=ul.LoginProvider)) //get all external auth schemes that are not current login provider
                .ToList();
            string passwordHash=null;
            if(_userStore is IUserPasswordStore<AppUser> passwordStore){ // Check _userStore is instance of IUserPasswordStore
                passwordHash=await passwordStore.GetPasswordHashAsync(user,new System.Threading.CancellationToken());
            }
            bool showRemoveButton=passwordHash!=null || currentLogin.Count>1;
            ViewBag.currentLogin=currentLogin;
            ViewBag.otherLogins=otherLogins;
            ViewBag.showRemoveButton=showRemoveButton;
            return View();

        }

        //Post: /Manager/ExteraLogin/RemoveLogin
        [HttpPost("/externalLogin/removelogin/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(string loginProvider, string providerKey){
            var user =await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            var result =await _userManager.RemoveLoginAsync(user,loginProvider,providerKey);
            if(!result.Succeeded){
                ErrorMessage="The external Login was not removed";  
                return RedirectToAction();
            }
            await _signInManager.RefreshSignInAsync(user);  
            StatusMessasge="The external login was removed";
            return RedirectToAction();
        }

        //Post: /Manager/ExternalLogin/LinkLogin
        [HttpPost("externallogin/linklogin/")]
        public async Task<IActionResult>LinkLogin(string provider){
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Yêu cầu một điều hướng đến trình cung cấp đăng nhập bên ngoài để bắt đầu kết nối đăng nhập với user hiện tại
            var redirectUrl=Url.Action("LinkLoginCallback","Manager");
            var properties =_signInManager.ConfigureExternalAuthenticationProperties(provider,redirectUrl,_userManager.GetUserId(User));
            return new ChallengeResult(provider,properties);
        }

        //Get: /Manager/ExternalLogin.LinkLoginCallback
        [HttpGet("/externallogin/linklogincallback/")]
        public async Task<IActionResult> LinkLoginCallback(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User) );
            }
            var userId=_userManager.GetUserId(User);
            var info=await _signInManager.GetExternalLoginInfoAsync(userId); 
            if(info==null){
                throw new ApplicationException("Error loading external login information during link to login");
            }
            var result =await _userManager.AddLoginAsync(user,info);
            if (!result.Succeeded){
                ErrorMessage="The external login was not added. Error: "+result.Errors.FirstOrDefault()?.Description;
                return RedirectToAction();
            }
            //Clear the existing external cookie to ensure a clean login proccess
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);  
            StatusMessasge="The external login was added";
            return RedirectToAction();

        }
              

    }
}