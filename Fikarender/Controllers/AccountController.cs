using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fikarender.Data;
using Fikarender.Models;
using Helpers;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using reCAPTCHA.AspNetCore;
using reCAPTCHA.AspNetCore.Attributes;
using ResizeImageASPNETCore;
using X.PagedList;

namespace Fikarender.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IMemoryCache _memoryCache;
        //private readonly SMSSender _smsSender;
        private readonly IEmailSender _emailSender;
        private readonly IRecaptchaService _recaptcha;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            ApplicationDbContext context,
            IMemoryCache memoryCache,
            //SMSSender smsSender,
            IEmailSender emailSender,
            IRecaptchaService recaptcha,
            IWebHostEnvironment environment,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            db = context;
            _memoryCache = memoryCache;
            _recaptcha = recaptcha;
            //_smsSender = smsSender;
            _emailSender = emailSender;
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /*#region Authentication

        #region Login & Logout

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            TempData["msg"] = "با موفقیت از سیستم خارج شدید. |success";

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM Input)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                Input.Password = Input.Password.PersianToEnglish();
                Input.Username = Input.Username.PersianToEnglish();
                var result = await _signInManager.PasswordSignInAsync(userName: Input.Username, password: Input.Password, isPersistent: Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(Input.Username);

                    #region getting basket from cookie and move to user

                    *//*// getting basket from cookie and move to user
                    HttpContext.Request.Cookies.TryGetValue("parsmvcBasket", out string cookie);
                    if (cookie != null)
                    {
                        var cookieBasket = await db.Order.SingleOrDefaultAsync(a => a.CookieValue.Equals(cookie) && a.Status.Equals((byte)OrderStatus.Initial));
                        if (cookieBasket != null)
                        {
                            if (cookieBasket.Status.Equals(0) && cookieBasket.LastUpdate < DateTime.Now.AddDays(-30))
                            {
                                db.Remove(cookieBasket);
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                var userBasket = await db.Order.SingleOrDefaultAsync(a => a.UserId.Equals(user.Id) && a.CookieValue != cookie);
                                if (userBasket != null)
                                {
                                    if (userBasket.Status.Equals((byte)OrderStatus.Initial) && userBasket.LastUpdate < DateTime.Now.AddDays(-30))
                                    {
                                        db.Remove(userBasket);
                                        await db.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        var productsInUserBasket = userBasket.OrderDetails.Select(a => a.ProductId).ToList();
                                        foreach (var item in cookieBasket.OrderDetails)
                                        {
                                            if (productsInUserBasket.Contains(item.ProductId))
                                            {
                                                var i = userBasket.OrderDetails.Where(a => a.ProductId.Equals(item.ProductId)).Single();
                                                db.Remove(i);
                                            }
                                            item.OrderId = userBasket.Id;
                                            db.Update(item);
                                        }
                                        userBasket.CookieValue = cookie;
                                        userBasket.LastUpdate = cookieBasket.LastUpdate;
                                        db.Update(userBasket);
                                        try
                                        {
                                            await db.SaveChangesAsync();
                                            db.Remove(cookieBasket);
                                            await db.SaveChangesAsync();
                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }
                                    }
                                }
                                else
                                {
                                    if (cookieBasket.UserId != user.Id)
                                    {
                                        cookieBasket.UserId = user.Id;
                                        db.Update(cookieBasket);
                                        try
                                        {
                                            await db.SaveChangesAsync();
                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // end*//*

                    #endregion

                    var returnUrl = Request.Form["returnUrl"].ToString();
                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    var user = await _userManager.FindByNameAsync(Input.Username);
                    if (!string.IsNullOrWhiteSpace(user.LockoutReason))
                    {
                        TempData["msg"] = user.LockoutReason + " |danger";
                    }
                    return RedirectToAction("Lockout");
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(Input.Username);
                    if (!await _userManager.IsPhoneNumberConfirmedAsync(user))
                    {
                        TempData["msg"] = "شماره تلفن همراه شما تایید نشده است. |danger";
                        return RedirectToAction("SendNewCode", new { phoneNumber = Input.Username });
                    }
                    ModelState.AddModelError(string.Empty, "خطای ورود به سیستم! شماره تلفن همراه یا کلمه عبور را اشتباه وارد کرده باشید.");
                }
            }
            return View();
        }

        #endregion

        #region register

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateRecaptcha]
        public async Task<IActionResult> Register(RegisterVM Input)
        {
            if (ModelState.IsValid)
            {
                Input.Username = Input.Username.PersianToEnglish();
                var user = new ApplicationUser
                {
                    *//*Gender = Input.Gender,
                    UserName = Input.Username,
                    Firstname = Input.Firstname,
                    Lastname = Input.Lastname,
                    PhoneNumber = Input.Username,
                    PhoneNumberConfirmed = false,
                    RegisterDate = DateTime.Now*//*
                };

                Input.Password = Input.Password.PersianToEnglish();
                Input.ConfirmPassword = Input.ConfirmPassword.PersianToEnglish();
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, Input.Username);
                    var res = *//*await _smsSender.SendPattern(user.PhoneNumber, code, "activate");*//* "ok";
                    if (res.Equals("ok"))
                    {

                        TempData["Code"] = code;

                        _memoryCache.Set("PhoneNumber", user.PhoneNumber, new MemoryCacheEntryOptions { Priority = CacheItemPriority.High, SlidingExpiration = TimeSpan.FromMinutes(20) });
                        return RedirectToAction("ConfirmPhoneNumber");
                    }
                    else
                    {
                        TempData["msg"] = "به علت شلوغی خطوط پیامک تایید برای شما ارسال نشد. لطفا دوباره تلاش کنید. |danger";
                        return RedirectToAction("SendNewCode", new { phoneNumber = user.PhoneNumber });
                    }
                }
                foreach (var error in result.Errors)
                {
                    if (error.Code.Equals("PasswordRequiresLower"))
                    {
                        error.Description = "کلمه عبور باید حاوی یک حرف انگلیسی کوچک باشد ('a'-'z')";
                    }
                    if (error.Code.Equals("PasswordRequiresDigit"))
                    {
                        error.Description = "کلمه عبور باید حاوی یک عدد باشد ('0'-'9')";
                    }
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        #endregion

        #region Confirmation & Send Code

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmPhoneNumber()
        {
            var mobile = _memoryCache.Get("PhoneNumber");
            if (mobile != null)
            {
                ViewData["PhoneNumber"] = (string)mobile;
                return View();
            }
            else
            {
                return RedirectToAction("SendNewCode");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmPhoneNumber(ConfirmPhoneNumberVM Input)
        {
            if (ModelState.IsValid)
            {
                Input.Code = Input.Code.PersianToEnglish();
                Input.PhoneNumber = Input.PhoneNumber.PersianToEnglish();
                var user = await _userManager.FindByNameAsync(Input.PhoneNumber);
                var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, Input.Code);
                if (result.Succeeded)
                {
                    user.ConfirmationDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["msg"] = "شماره موبایل شما تایید شد. لطفا وارد شوید. |success";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["msg"] = "عملیات با خطا مواجه شد. خطا در برقراری ارتباط با سرور. |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. کد وارد شده صحیح نیست. |danger";
                }
            }
            TempData["msg"] = "درخواست شما غیر مجاز است. |danger";

            var mobile = _memoryCache.Get("PhoneNumber");
            if (mobile != null)
            {
                ViewData["PhoneNumber"] = (string)mobile;
                return View();
            }
            else
            {
                return RedirectToAction("SendNewCode");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendNewCode(string phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                ViewData["phoneNumber"] = phoneNumber;
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendNewCode(SendNewCodeVM Input)
        {
            if (ModelState.IsValid)
            {
                Input.PhoneNumber = Input.PhoneNumber.PersianToEnglish();
                var user = await _userManager.FindByNameAsync(Input.PhoneNumber);
                if (user != null)
                {
                    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

                    var res = *//*await _smsSender.SendPattern(user.PhoneNumber, code, "activate");*//* "ok";
                    if (res.Equals("ok"))
                    {

                        TempData["Code"] = code;

                        _memoryCache.Set("PhoneNumber", user.PhoneNumber, new MemoryCacheEntryOptions { Priority = CacheItemPriority.High, SlidingExpiration = TimeSpan.FromMinutes(20) });
                        ViewData["PhoneNumber"] = Input.PhoneNumber;
                        return RedirectToAction("ConfirmPhoneNumber");
                    }
                }
            }
            TempData["msg"] = "درخواست شما غیر مجاز است. |danger";
            return View();
        }

        #endregion

        #region Forgot Password

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(SendNewCodeVM Input)
        {
            if (ModelState.IsValid)
            {
                Input.PhoneNumber = Input.PhoneNumber.PersianToEnglish();
                var user = await _userManager.FindByNameAsync(Input.PhoneNumber);
                if (user == null || !(await _userManager.IsPhoneNumberConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("ResetPassword");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var res = *//*await _smsSender.SendPattern(user.PhoneNumber, code, "reset")*//* "ok";
                if (res.Equals("ok"))
                {

                    TempData["Code"] = code;

                    return RedirectToAction("ResetPassword");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713

                //var callbackUrl = Url.Page(
                //    "/Account/ResetPassword",
                //    pageHandler: null,
                //    values: new { code },
                //    protocol: Request.Scheme);

                //await _emailSender.SendEmailAsync(
                //    Input.PhoneNumber,
                //    "Reset Password",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }
            return View();
        }

        #endregion

        #region Reset password

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM Input)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Input.PhoneNumber = Input.PhoneNumber.PersianToEnglish();
            Input.Code = Input.Code.PersianToEnglish();
            Input.Password = Input.Password.PersianToEnglish();

            var user = await _userManager.FindByNameAsync(Input.PhoneNumber);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Change Password

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM m)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            m.OldPassword = m.OldPassword.PersianToEnglish();
            m.NewPassword = m.NewPassword.PersianToEnglish();
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, m.OldPassword, m.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    if (error.Code.Equals("PasswordRequiresLower"))
                    {
                        error.Description = "کلمه عبور باید حاوی یک حرف انگلیسی کوچک باشد ('a'-'z')";
                    }
                    if (error.Code.Equals("PasswordRequiresDigit"))
                    {
                        error.Description = "کلمه عبور باید حاوی یک عدد باشد ('0'-'9')";
                    }
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["msg"] = "کلمه عبور شما با موفقیت تغییر یافت. |success";

            return View();
        }

        #endregion

        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion*/

        public IActionResult Index()
        {
            return View();
        }
    }
}
