﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OMS_App.Models;
using OMS_App.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Cryptography.Xml;
using System.Security.Claims;
using System.Security;
using OMS_App.Data;

namespace OMS_App.Areas.Identity.Controllers
{
    [Authorize]
    [Area("Identity")]

    public class AccountController : Controller
    {
        private readonly IUserImageRepo _userImageRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserStore<AppUser> _userStore;
        // private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IUserStore<AppUser> userStore, IEmailSender emailSender, IUserImageRepo userImageRepo)
        {



            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _userStore = userStore;
            //_emailStore = emailStore;
            _emailSender = emailSender;
            _userImageRepo = userImageRepo;
        }
        [TempData]
        public string ErrorMessage { get; set; }

        //Get: /Account/register/url
        [HttpGet]
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
                var user = new AppUser()
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                // await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                // await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
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
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterConfirmation(string email, string returnUrl = null, bool displayConfirmationAccountLink = false)
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
            if (!displayConfirmationAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var EmailConfirmationUrl = Url.Action(
                    "ConfirmEmail",
                    controller: "Account",
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
                ViewBag.EmailConfirmationUrl = EmailConfirmationUrl;
                _logger.LogInformation("Url:" + EmailConfirmationUrl);
            }
            return View();
        }

        //Get: /Acount/Login/url
        [HttpGet]
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
            var ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())?.ToList();
            ViewBag.ExternalLogins = ExternalLogins;
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        //POST: Account/login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");// Return absolute path of url
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                ViewBag.StatusMessage = "Can not to login";
                return View();

            }
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
            return RedirectToAction("Index", "Home");


        }

        //Get: /Account/LoginWith2fa
        [HttpGet]
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
        [HttpPost]
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
        [HttpGet]
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
        [HttpPost]
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
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin()
        {
            return RedirectToAction("Index");
        }


        //Post: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", controller: "Account", values: new { area = "Identity", returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }



        //Get: /Account/ExternalLoginCallBack
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }
            // Lấy thông tin user từ ứng dụng ngoài trả về callback.
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }
            Console.WriteLine("Login Provider: " + info.LoginProvider);
            Console.WriteLine("Login key:" + info.ProviderKey);

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {

                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }
            else
            {
                //Kiểm tra user đã có tài khoản, chưa đăng nhập được là do chưa xác thực email
                var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (existingUser != null)
                {
                    _logger.LogInformation("user has account but not confirm");
                    return RedirectToAction("RegisterConfirmation", "Account");
                }
                _logger.LogInformation("user does not have account, ask user create an account");
                // If the user does not have an account, then ask the user to create an account.
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ProviderDisplayName = info.ProviderDisplayName;
                // Check if external email matches the email of any previous logged the user
                ExternalLoginViewModel externalLogin;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    externalLogin = new ExternalLoginViewModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };

                }
                else
                {
                    externalLogin = externalLogin = new ExternalLoginViewModel
                    {
                        Email = String.Empty
                    };
                }
                return View(externalLogin);


            }


        }
        // Post: Account/confirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmationAsync(ExternalLoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            // lấy lại thông tin provider đăng nhập trên cookies
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToAction("Index", new { returnUrl });
            }
            _logger.LogInformation("Already get provider information");
            if (ModelState.IsValid)
            {
                var user = new AppUser()
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                // Set username and email for user
                // await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None );
                // await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None );
                _logger.LogInformation("Confirmation: create user");
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    //Add user login information
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (addLoginResult.Succeeded)
                    {
                        _logger.LogInformation("Confirmation: create user successfully");
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            controller: "Account",
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme

                            );

                        await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                            $"Please confirm your account by >a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>click here.");
                        _logger.LogInformation("Sent email confirmation", code.ToString());
                        // if account confirmation is required, we need to show the link if we don't have a real email
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToAction("RegisterConfirmation", new { email = model.Email, returnUrl = returnUrl });

                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        _logger.LogInformation("Areadly loggin");
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        _logger.LogInformation("Confirmation: Can no create user successfully");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }


            }
            ViewBag.ProviderDisplayName = info.ProviderDisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalLoginCallBack");
        }

        [HttpGet]

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            Console.WriteLine("Come to get logout");
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");

            return View();

        }
        //Get: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutAync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");

            return View();

        }
        //Get: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            if (userId == null || code == null) return RedirectToAction("/Index");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                var userImage = new UserImage();
                userImage.AppUserId = userId;
                userImage.ImagePath = "~/files/userimages/user-default.png";
                userImage.IsActive = true;
                _userImageRepo.CreateUserImageAsync(userImage);
                ViewBag.StatusMessage = "Thank you for confirming your email.";
            }
            else
            {
                ViewBag.StatusMessage = "Error confirming your email."; ;
            }
            return View();
        }

        //Get: //Account/ConfirmEmailChange
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailChangeAsync(string userId = null, string email = null, string code = null)
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
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }


        //Post: /Account/ResendEmailConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmationAsync(ResendEmailConfirmationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                       "ConfirmEmail",
                       controller: "Account",
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
        [HttpPost]
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
                    return RedirectToAction("ForgotPasswordConfirmation");
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
                return RedirectToAction("ForgotPasswordConfirmation");

            }
            return View();
        }


        //Get:/Account/forgotpasswordconfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //Get:/Account/Resetpassword
        [HttpGet("/resetpassword/")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset");
            }
            else
            {
                var resetPasswordViewModel = new ResetPasswordViewModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return View();
            }
        }

        //Post:/Account/Resetpassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("ResetPasswordConfirmation");
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
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
        [HttpGet]
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
