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
using System.Security.Cryptography.Xml;
using System.Security.Claims;
using System.Security;
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
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IUserStore<AppUser> userStore, IUserEmailStore<AppUser> emailStore, IEmailSender emailSender)
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
                        controller: "AccountController",
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
        public async Task< IActionResult> RegisterConfirmation(string email, string returnUrl = null, bool displayConfirmationAccountLink = false)
        {
            ViewBag.DisplayConfirmationAccountLink = displayConfirmationAccountLink;
            if (email == null)
            {
                return RedirectToAction("/Index");
            }
            returnUrl ??= Url.Content("~/");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("Can not find out user have email: " + email);
            // Create a code confirmation when use fake email. When you use real email please remove this code or pass value "false" to displayConfirmationAccountLink
            if (displayConfirmationAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
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
        public async Task<IActionResult> LoginWith2Fa(bool rememerMe, string returnUrl = null)
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
        public async Task<IActionResult> LoginWith2Fa(LoginWith2faViewModel model, bool rememerMe, string returnUrl = null)
        {
            ViewBag.RemeberMe = rememerMe;
            ViewBag.ReturnUrl = returnUrl;
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
        public IActionResult LoginWithRecoveryCode(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            var user = _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Unable to load two-factor authentication user");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //Post: /Account/loginwithrecoverycode
        [HttpPost("/loginwithrecoverycode/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Unable to load two factor authentication user");
            }
            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
            var userId = await _userManager.GetUserIdAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with Id '{UserId}' logged in with a recovery code.", user.Id);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogInformation("User with Id '{UserId}' was locked out", user.Id);
                return RedirectToAction("Lockout");

            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();

            }

        }

        //Get: /Account/ExternalLogin
        [HttpGet("/externallogin/")]
        [AllowAnonymous]
        public IActionResult ExternalLogin()
        {
            return RedirectToAction("Index");
        }


        //Post: /Account/ExternalLogin
        [HttpPost("/externallogin/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLogin", controller: "AccountController", values: new { area = "Identity", returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
           
            return new ChallengeResult(provider, properties);
        }



        //Get: /Account/ExternalLoginCallBack
        [HttpGet("/externallogincallback/")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack(ExternalLoginViewModel model, string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError == null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = $"Error loading external login information";
                return RedirectToAction("Login", new { returnUrl = returnUrl });
            }
            //Sign in user with this external login provider if the user already has a login
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvide} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }
            else
            {
                // if the user does not have an account, then ask the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    ViewBag.ExternalLoginViewModel = new ExternalLoginViewModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };


                }
                return View();
            }

        }
        // Post: Account/confirmation
        [HttpPost("/confirmation/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmationAsync(ExternalLoginViewModel model,string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            // get user information from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToAction("Index", new { returnUrl });
            }
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                // Set username and email for user
                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None );
                await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None );
                var result =await  _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                    var userId =await _userManager.GetUserIdAsync(user);
                    var code=await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code=WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        controller: "AccountController",
                        values: new { area = "Identity", userId = userId, code = code },
                        protocol: Request.Scheme

                        );
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by >a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>click here.");
                    // if account confirmation is required, we need to show the link if we don't have a real email
                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        return RedirectToAction("RegisterConfirmation",new {Email=model.Email});

                    }
                    await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider); 
                    return LocalRedirect(returnUrl);
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.ProviderDisplayName = info.ProviderDisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        //Get: /Account/Logout
        [HttpGet("/logout/")]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                //this need to be a redirect so that the browser performs a new request and the identity for the user gets updated
                return RedirectToAction();
            }
        }
        //Get: /Account/ConfirmEmail
        [HttpGet("/confirmemail/")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
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
        public async Task<IActionResult> ConfirmEmailChangeAsync(string userId=null, string email=null, string code=null)
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
        [AllowAnonymous]
        public IActionResult ResendEmailConfirmation()
        {
          return View();
        }


        //Post: /Account/ResendEmailConfirmation
        [HttpPost("/resendemailconfirmation/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmationAsync(ResendEmailConfirmationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user=await _userManager.FindByEmailAsync(model.Email);
            var code =await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code=WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                       "ConfirmEmail",
                       controller: "AccountController",
                       values: new { area = "Identity", userId = user.Id, code = code },
                       protocol: Request.Scheme

                       );
            await _emailSender.SendEmailAsync(
               model.Email,
               "Confirm your email",
               $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
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
                    controller: "AccountController",
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
        public IActionResult ResetPassword( string code=null)
        {
            if(code== null)
            {
                return BadRequest("A code must be supplied for password reset");
            }
            else
            {
                var resetPasswordViewModel = new ResetPasswordViewModel{
                    Code=Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return View();
            }
        }

        //Post:/Account/Resetpassword
        [HttpPost("/resetpassword/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) 
            {
                return View();
            }
            var user= await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("ResetPasswordConfirmation");
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
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
