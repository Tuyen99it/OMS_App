using Microsoft.AspNetCore.Authentication;
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
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return View(model);
                }
               
            }
            return View(model);


        }

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
            returnUrl ??=Url.Content("~/");
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
                        "/Account/ConfirmEmail",
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
                        return RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl = returnUrl });

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
                // If we got this far, something failed, redisplay form
                return View();
            }
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
