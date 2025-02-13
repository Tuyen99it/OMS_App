﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using OMS_Webapp.Models;
using OMS_Webapp.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
namespace OMS_Webapp.Areas.Identity.Controllers
{
    [Authorize]
    [Area("Identity")]
    [Route("/Account/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IUserStore<AppUser> userStore, IUserEmailStore<AppUser> emailStore,IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _userStore = userStore;
            _emailStore = emailStore;
            _emailSender = emailSender;
        }
        [TempData]
        public string ErrorMessage { get; set; }

        //Get: /Account/register/url
        [HttpGet("/register/")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl ??= Url.Content("~/"); // if return ==null, returnUrl= absolute path
            // clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //POST: /Account/register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password");
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "/ConfirmEmail",
                        controller: "Account",
                        values: new
                        {
                            area = "Identity",
                            userId = userId,
                            code = code,
                            returnUrl = returnUrl
                        },
                        protocol: Request.Scheme
                    );
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", new { email = model.Email, returnUrl = returnUrl });

                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        //Get: /Account/RegisterConfirmation: Allow confirm register when using fake email addres
        [HttpGet("/registerconfirmation/")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterConfirmation( string email,string returnUrl=null,bool displayConfirmationAccountLink=false)
        {
           if(email == null)
            {
                return RedirectToAction("/Index");
            }
            returnUrl ??= Url.Content("~/");
            var user=await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("Can not find out user have email: " + email);
            // Create a code confirmation when use fake email. When you use real email please remove this code or pass value "false" to displayConfirmationAccountLink
            if (displayConfirmationAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code=await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code=WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                ViewBag.EmailConfirmationUrl = Url.Action(
                    "ConfirmEmail",
                    controller: "Account",
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }
            return View();  
        }

        //Get: /Acount/Login/url
        [HttpGet("/login/")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl ??= Url.Content("~/"); // if return ==null, returnUrl= absolute path
            // clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        //POST: Account/login
        [HttpPost("/login/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");// Return absolute path of url
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                // login by Email
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    // login by user
                    var userName = await _userManager.FindByEmailAsync(model.UserName);
                    if (userName != null)
                    {
                        result = await _signInManager.PasswordSignInAsync(userName.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                    }

                }
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, Remember = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "Account is locked");
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return View(model);
                }

            }
            return View(model);


        }

        //Get: /Account/LoginWith2fa
        [HttpGet("/loginwith2fa/")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2Fa(bool rememerMe,string returnUrl=null)
        {
            // Ensure the user has gon through the username and password screem first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Unable to load two-factor authentication user");
            }
            ViewBag.ReturnUrl = returnUrl;  
            ViewBag.RememberMe = rememerMe;
            return View();
        }

        //Post: / Account/LoginWith2Fa
        [HttpPost("/loginwith2fa/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2Fa(LoginWith2faViewModel model,bool rememerMe,string returnUrl=null )
        {
            if (!ModelState.IsValid) return View();
            returnUrl ??= Url.Content("~/");
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }
            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememerMe, model.RememberMachine);
            var userId = await _userManager.GetUserIdAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToAction("Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
            
        }

        //Get: /Account/LoginWithRecoveryCode/
        [HttpGet("/loginwithrecoverycode/")]
        public IActionResult LoginWithRecoveryCode()
        {
            return View();
        }

        //Post: /Account/loginwithrecoverycode
        [HttpPost("/loginwithrecoverycode/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl=null)
        {
            return View();
        }

        //Get: /Account/ExternalLogin
        [HttpGet("/externallogin/")]
        public async Task<IActionResult> ExternalLogin()
        {
            return View();
        }


        //Post: /Account/ExternalLogin/callbackUrl
        [HttpPost("/externallogin/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLogin(string callbackUrl)
        {
            return View();
        }

        //Get: /Account/Logout
        [HttpGet("/logout/")]
        public async Task<IActionResult> Logout()
        {
            return View();
        }


        //Post: /Account/Logout
        [HttpPost("/logout/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            return View();
        }

        //Get: /Account/ConfirmEmail
        [HttpGet("/confirmemail/")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null) return RedirectToAction("/Index");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            ViewBag.StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            return View();
        }

        //Get: //Account/ConfirmEmailChange
        [HttpGet("/confirmemailchange/")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToAction("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                ViewBag.StatusMessage = "Error changing email.";
                return View();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                ViewBag.StatusMessage = "Error changing user name.";
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            ViewBag.StatusMessage = "Thank you for confirming your email change.";
            return View();
        }

        //Get: /Account/ResendEmailConfirmation
        [HttpGet("/ResendEmailConfirmation/")]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            return View();
        }


        //Get:/Account/ForgetPassword
        [HttpGet("/forgotpassword/")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            return View();
        }


        //Post: /Account/forgotpassword/
        [HttpPost("/forgotpassword/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("./ForgotPasswordConfirmation");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                _logger.LogInformation("Code:{code}", code);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                _logger.LogInformation("Encode:{code}", code);
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    controller: "Account",
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "ResetPassword", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' clicking here</a>");
                return RedirectToAction("./ForgotPasswordConfirmation");

            }
            return View();
        }


        //Get:/Account/forgotpasswordconfirmation
        [HttpGet("/forgotpasswordconfirmation/")]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //Get:/Account/Resetpassword
        [HttpGet("/resetpassword/")]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }

        //Get:/Account/ResetpasswordConfirmation
        [HttpGet("/resetpasswordconfirmation/")]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //Get:/Account/Lockout
        [HttpGet("/lockout/")]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        // Create AppUser
        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                     $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                     $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }


      
    }
}
