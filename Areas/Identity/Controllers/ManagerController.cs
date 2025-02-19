using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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



              

    }
}