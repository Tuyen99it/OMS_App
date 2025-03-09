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
using Microsoft.AspNetCore.Mvc.Rendering;
using OMS_App.Areas.Identity.Models;
using OMS_App.Models;
using Microsoft.AspNetCore.Authorization;

namespace OMS_App.Areas.Identity.Controllers
{
    [Area("Identity")]
   
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
        public string StatusMessage { get; set; }
        [HttpGet]
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
        
        [HttpPost]
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

        [HttpGet]
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
            var isEmailConfirm=await _userManager.IsEmailConfirmedAsync(user);
            ViewBag.isEmailConfirm=isEmailConfirm;
            return View();

        }

        [HttpPost]
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

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user=await _userManager.GetUserAsync(User);
            if(user==null)
            {
                return RedirectToAction("Login","Account");
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
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

        [HttpGet]
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
        [HttpPost]
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
            StatusMessage="The external login was removed";
            return RedirectToAction();
        }

        //Post: /Manager/ExternalLogin/LinkLogin
        [HttpPost]
        public async Task<IActionResult>LinkLogin(string provider){
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Yêu cầu một điều hướng đến trình cung cấp đăng nhập bên ngoài để bắt đầu kết nối đăng nhập với user hiện tại
            var redirectUrl=Url.Action("LinkLoginCallback","Manager");
            var properties =_signInManager.ConfigureExternalAuthenticationProperties(provider,redirectUrl,_userManager.GetUserId(User));
            return new ChallengeResult(provider,properties);
        }

        //Get: /Manager/ExternalLogin.LinkLoginCallback
        [HttpGet]
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
            StatusMessage="The external login was added ";
            return RedirectToAction();

        }

        //Get: /Manager/TowFactorAuthentication
        public async Task<IActionResult> TwoFactorAuthentication(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id:"+_userManager.GetUserId(User));
            }
            bool hasAuthenticator=await _userManager.GetAuthenticatorKeyAsync(user)!=null;
            bool is2FaEnabled=await _userManager.GetTwoFactorEnabledAsync(user);
            bool isMachineRemembered=await _signInManager.IsTwoFactorClientRememberedAsync(user);
            var recoveryCodesLeft=await _userManager.CountRecoveryCodesAsync(user);
            ViewBag.HasAuthenticator=hasAuthenticator;
            ViewBag.Is2FaEnabled=is2FaEnabled;
            ViewBag.IsMachineRemembered=isMachineRemembered;
            ViewBag.RecoveryCodesLeft=recoveryCodesLeft;
            return View();

        }

        //Post: /Manager/TwoFactorAuthentication
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> TwoFactorAuthenticationAsync(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id:"+_userManager.GetUserId(User));
            }
            await _signInManager.ForgetTwoFactorClientAsync();
            StatusMessage="The current browser has been forgotten. When you login again from this browser, you will be prompted for your 2fa code.";
            return RedirectToAction();
          
        }
        //Get: /Manager/EnableAuthenticator
        public async Task<IActionResult>EnableAuthenticator(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            await LoadSharedKeyAndQrCodeUriAsync(user);
            return View();
        }

        //Post: /Manager/EnableAuthenticator
        [HttpPost]
        public async Task<IActionResult> EnableAuthenticator(Enable2FaAuthenticationViewModel model){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            if(!ModelState.IsValid){
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return View();
            }
            var verificationCode=model.Code.Replace(" ","").Replace("-","");
            var is2FaTokenValid=await _userManager.VerifyTwoFactorTokenAsync(user,_userManager.Options.Tokens.AuthenticatorTokenProvider,verificationCode);
            if(!is2FaTokenValid){
                ModelState.AddModelError(model.Code,"Verification code is invalid");
                return View();
            }
            await _userManager.SetTwoFactorEnabledAsync(user,true);
            var userId=await _userManager.GetUserIdAsync(user);
            _logger.LogInformation("User with Id {UserId} has enabled 2FA with an authenticator app.",userId);
            StatusMessage="Your authenticator app has been verified";
            if(await _userManager.CountRecoveryCodesAsync(user)==0){
                var recoveryCodes=await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user,10);
                TempData["RecoveryCodes"]=recoveryCodes.ToArray();
                return RedirectToAction("ShowRecoveryCodes");
            } 
            else{
                return RedirectToAction("TwoFactorAuthentication");
            }  
        }
        
        // Load the shared key and Qr codel Uri to enable authenticator app
        private async Task LoadSharedKeyAndQrCodeUriAsync(AppUser user){
            // load the authenticator key and Qr code Uri to display on the form
            var unformattedKey=await _userManager.GetAuthenticatorKeyAsync(user);
            if(string.IsNullOrEmpty(unformattedKey)){
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey=await _userManager.GetAuthenticatorKeyAsync(user);
            }
            var sharedKey=FormatKey(unformattedKey);
            var email=await _userManager.GetEmailAsync(user);
            var authenticatorUri=GenerateQrCodeUri(email,unformattedKey);
            ViewBag.SharedKey=sharedKey;
            ViewBag.AuthenticatorUri=authenticatorUri;
        
        }
        //Format the shared key to display in Qr code
        private string FormatKey(string unformattedKey){
            var result=new System.Text.StringBuilder();
            int currentPosition=0;
            while(currentPosition+4<unformattedKey.Length){
                result.Append(unformattedKey.AsSpan(currentPosition,4)).Append(" ");
                currentPosition+=4;
            }
            if(currentPosition<unformattedKey.Length){
                result.Append(unformattedKey.AsSpan(currentPosition));
            }
            return result.ToString().ToLowerInvariant();
            
        }

        //Generate the Qr code Uri
        private string GenerateQrCodeUri(string email,string unformattedKey){
            return string.Format(
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                System.Web.HttpUtility.UrlEncode("OMS_App"),
                System.Web.HttpUtility.UrlEncode(email),
                unformattedKey); 
        }   

        //Get: /Manager/Disable2Fa
        public async Task<IActionResult> Disable2Fa(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            if(!await _userManager.GetTwoFactorEnabledAsync(user)){
                throw new ApplicationException($"Cannot disable 2FA for user with ID '{_userManager.GetUserId(User)}' as it's not currently enabled.");
            }
            return View();
        }

        //Post: /Manager/Disable2Fa

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2FaAsync(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            var disable2FaResult=await _userManager.SetTwoFactorEnabledAsync(user,false);
            if(!disable2FaResult.Succeeded){
                throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{_userManager.GetUserId(User)}'.");
            }
            _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.",_userManager.GetUserId(User));
            StatusMessage="2FA has been disabled. You can reenable 2FA at any time by reconfiguring the authenticator app.";
            return RedirectToAction("TwoFactorAuthentication");
        }

        //Get: /Manager/ResetAuthenticator
        public async Task<IActionResult> ResetAuthenticator(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            return View();
        }

        //Post: /Manager/ResetAuthenticator
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>ResetAuthenticatorAsync(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            await _userManager.SetTwoFactorEnabledAsync(user,false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with ID '{UserId}' has reset their authenticator app key.",_userManager.GetUserId(User));
            return RedirectToAction("EnableAuthenticator");
        }

        //Get: /Manager/ShowRecoveryCodes
        public IActionResult ShowRecoveryCodes(){
            var recoveryCodes= (string[])TempData["RecoveryCodes"];
            if(recoveryCodes==null||recoveryCodes.Length==0){
                return RedirectToAction("TwoFactorAuthentication");
            }
            TempData["RecoveryCodes"]=recoveryCodes;
            return View();
        }

        //Get: /Manager/GenerateRecoveryCodes
        public async Task<IActionResult> GenerateRecoveryCode(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            var is2FaEnabled=await _userManager.GetTwoFactorEnabledAsync(user);
            if(!is2FaEnabled){
                throw new InvalidOperationException($"Cannnot generate recovery codes for user with Id '{_userManager.GetUserId(User)}' as they do not have 2FA enabled.");
            }
            return View();
        }

        //Post: /Manager/GenerateRecoveryCodes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>GenerateRecoveryCodesAync(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            var is2FaEnabled=await _userManager.GetTwoFactorEnabledAsync(user);
            if(!is2FaEnabled){
                throw new InvalidOperationException($"Cannnot generate recovery codes for user with Id '{_userManager.GetUserId(User)}' as they do not have 2FA enabled.");
            }
            var recoveryCodes=await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user,10);
            ViewBag.RecoveryCodes=recoveryCodes.ToArray();
            _logger.LogInformation("User with Id '{UserId}' has generated new 2FA recovery codes.",_userManager.GetUserId(User));
            StatusMessage="You have generated new recovery codes.";
            return RedirectToAction("ShowRecoveryCodes");
        }

        //Post: /Manager/DownloadPersonData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadPersonalDataAsync(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
           _logger.LogInformation("User with Id '{UserId}' asked for their personal data.",_userManager.GetUserId(User));
            
            //Only include personal data for download
            var personalData=new Dictionary<string,string>();   
            var personalDataProps=typeof(AppUser).GetProperties().Where(
                prop=>Attribute.IsDefined(prop,typeof(PersonalDataAttribute)));  
            foreach (var p in personalDataProps){
                personalData.Add(p.Name,p.GetValue(user)?.ToString()??"null");
            }
            personalData.Add("AuthenticatorKey",await _userManager.GetAuthenticatorKeyAsync(user)?? "null");
            Response.Headers.TryAdd("Content-Disposition","attachment;filename=PersonalData.json");
          
            return new FileContentResult(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(personalData),"application/json");     
        }

         //Get: Mamager/PersonalData
        public async Task<IActionResult> PersonalData(){
            var user =await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            return View();
        }

        //Get: /Manager/DeletePersonalData
        public async Task<IActionResult> DeletePersonalData(){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            return View();
        }

        //Post: /Manager/DeletePersonalData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePersonalDataAsync(DeletePersonalDataViewModel model){
            var user=await _userManager.GetUserAsync(User);
            if(user==null){
                return NotFound("Unable to load user with Id: "+_userManager.GetUserId(User));
            }
            var requuiredPassword=await _userManager.HasPasswordAsync(user);
            if(requuiredPassword){
                if(!await _userManager.CheckPasswordAsync(user,model.Password)){
                    ModelState.AddModelError(string.Empty,"Incorrect password.");
                    return View();
                }
            }
            
            var userId=_userManager.GetUserId(User);
            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);
            _logger.LogInformation("User with Id '{UserId}' deleted themselves.",userId);
            return RedirectToAction("Index","Home");
        }

    
    }
    public static class ManageNavView{
        public static string Index =>"Index";
        public static string Email =>"Email";
        public static string ChangePassword =>"ChangePassword";
        public static string ExternalLogin =>"ExternalLogin";
        public static string DownloadPersonData =>"DownloadPersonData";
        public static string PersonalData =>"PersonalData";
        public static string DeletePersonalData =>"DeletePersonalData";
        public static string TwoFactorAuthentication =>"TwoFactorAuthentication";
        public static string EnableAuthenticator =>"EnableAuthenticator";
        public static string Disable2Fa =>"Disable2Fa";
        public static string ResetAuthenticator =>"ResetAuthenticator";
        public static string GenerateRecoveryCodes =>"GenerateRecoveryCodes";
        public static string ShowRecoveryCodes =>"ShowRecoveryCodes";
        public static string IndexNavClass(ViewContext viewContext)=>PageNavClass(viewContext,Index);
        public static string EmailNavClass(ViewContext viewContext)=>PageNavClass(viewContext,Email);
        public static string ChangePasswordNavClass(ViewContext viewContext)=>PageNavClass(viewContext,ChangePassword);
        public static string ExternalLoginNavClass(ViewContext viewContext)=>PageNavClass(viewContext,ExternalLogin);
        public static string DownloadPersonDataNavClass(ViewContext viewContext)=>PageNavClass(viewContext,DownloadPersonData);
        public static string PersonalDataNavClass(ViewContext viewContext)=>PageNavClass(viewContext,   PersonalData);
        public static string DeletePersonalDataNavClass(ViewContext viewContext)=>PageNavClass(viewContext,DeletePersonalData);
        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext)=>PageNavClass(viewContext,TwoFactorAuthentication);           
        public static string EnableAuthenticatorNavClass(ViewContext viewContext)=>PageNavClass(viewContext,EnableAuthenticator);
        public static string Disable2FaNavClass(ViewContext viewContext)=>PageNavClass(viewContext  ,Disable2Fa);
        public static string ResetAuthenticatorNavClass(ViewContext viewContext)=>PageNavClass(viewContext ,ResetAuthenticator);
        public static string GenerateRecoveryCodesNavClass(ViewContext viewContext)=>PageNavClass(viewContext,GenerateRecoveryCodes);
        public static string ShowRecoveryCodesNavClass(ViewContext viewContext)=>PageNavClass(viewContext,ShowRecoveryCodes);

        public static string PageNavClass(ViewContext viewContext,string action){
            var activeAction=viewContext.ViewData["ActiveAction"] as string??System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activeAction,action,StringComparison.OrdinalIgnoreCase)?"active":"";

        }

    }

}