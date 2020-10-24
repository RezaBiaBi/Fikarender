using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fikarender.Data;
using Fikarender.Models;
using Helpers;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResizeImageASPNETCore;
using X.PagedList;


namespace Fikarender.Controllers
{
    /*[Authorize(Roles = "Admin")]*/
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ILinkTools _linkTools;
        private readonly IWebHostEnvironment _environment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            ILinkTools linkTools,
            IWebHostEnvironment environment,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            db = context;
            _linkTools = linkTools;
            _environment = environment;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CkEditor(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;

            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\upload")))
            {
                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\upload"));
            }

            string name = Path.GetRandomFileName();
            string ext = Path.GetExtension(upload.FileName).ToLower();

            var fileName = name + ext;
            var path = Path.Combine(_environment.WebRootPath, "img\\upload", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                upload.CopyTo(stream);
            }
            //-saving to database
            var uf = new UploadedFile
            {
                Name = name,
                Type = ext
            };
            db.UploadedFiles.Add(uf);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                System.IO.File.Delete(path);
                return StatusCode(404);
            }
            //-
            var url = $"{"/img/upload/"}{fileName}";

            return Json(new { uploaded = true, url });
        }

        [Route("{controller:slugify}/{action:slugify}/{color}")]
        public async Task<IActionResult> ChangeTheme(string color)
        {
            string userId = _userManager.GetUserId(User);
            var theme = await db.AdminTheme.SingleOrDefaultAsync(a => a.UserId.Equals(userId));
            if (theme != null)
            {
                theme.Theme = color;
                db.Update(theme);
                await db.SaveChangesAsync();
            }
            else
            {
                await db.AddAsync(new AdminTheme
                {
                    Theme = color,
                    UserId = userId
                });
            }

            HttpContext.Request.Cookies.TryGetValue("parsmvcTheme", out string cookie);
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                if (HttpContext.Request.Cookies.ContainsKey("parsmvcTheme"))
                {
                    HttpContext.Response.Cookies.Delete("parsmvcTheme");
                }
            }
            HttpContext.Response.Cookies.Append("parsmvcTheme", color, new CookieOptions
            {
                IsEssential = true,
                Expires = DateTimeOffset.Now.AddYears(1),
                HttpOnly = true,
                Path = HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase.ToString() : "/",
                Secure = HttpContext.Request.IsHttps
            });
            await db.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> UploadMultimedia(string type, IFormFile file)
        {
            string path = type switch
            {
                "logo" => Path.Combine(_environment.WebRootPath, "img\\logo.png"),
                _ => ""
            };
            try
            {
                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                TempData["msg"] = "فایل با موفقیت جایگزین شد. |success";
            }
            catch (Exception)
            {
                TempData["msg"] = "خطا در جایگزینی فایل. احتمال دارد فایل در حال استفاده باشد یا مجوز دسترسی در سرور صادر نشده باشد. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public JsonResult DeleteUploadedFile(int id)
        {
            var up = db.UploadedFiles.Find(id);
            string msg;
            string status;
            if (up != null)
            {
                string path = Path.Combine(_environment.WebRootPath, "img\\upload", up.Name + "" + up.Type);
                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        System.IO.File.Delete(path);
                    }
                    catch (Exception e)
                    {
                        msg = e.Message;
                        status = "danger";
                        return Json(new { msg, status });
                    }
                }
                db.UploadedFiles.Remove(up);
                try
                {
                    db.SaveChanges();
                    msg = "تصویر با موفقیت حذف شد.";
                    status = "success";
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    status = "danger";
                }
            }
            else
            {
                msg = "تصویر پیدا نشد!";
                status = "danger";
            }
            return Json(new { msg, status });
        }

        #region Gallery

        [HttpGet]
        public async Task<IActionResult> Gallery(int? pageNumber, int? serviceId)
        {
            if (serviceId.HasValue)
            {
                ViewData["serviceWorkSamples"] = await db.WorkSamples.Where(w => w.ServiceId.Equals(serviceId)).ToListAsync();

                var workSamples = db.WorkSamples.AsNoTracking().OrderByDescending(a => a.WorkSampleId);
                var pageNumb = pageNumber ?? 1;
                var onePageOfWorkSamples = await workSamples.ToPagedListAsync(pageNumb, 12);
                ViewBag.workSamples = onePageOfWorkSamples;
            }

            var data = db.UploadedFiles.AsNoTracking().OrderByDescending(a => a.Id);
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await data.ToPagedListAsync(pageNum, 24);
            ViewBag.data = onePageOfData;
            
            return View();
        }
        
        [HttpPost]
        public async Task<JsonResult> Gallery(List<IFormFile> images)
        {
            if (images == null)
            {
                return Json(false);
            }
            var validTypes = new string[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            foreach (var img in images)
            {
                if (validTypes.Contains(img.ContentType))
                {
                    if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\upload")))
                    {
                        Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\upload"));
                    }

                    string name = Path.GetRandomFileName();
                    string ext = Path.GetExtension(img.FileName).ToLower();
                    var fileName = name + ext;
                    var path = Path.Combine(_environment.WebRootPath, "img\\upload", fileName);
                    await using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }
                    var up = new UploadedFile
                    {
                        Name = name,
                        Type = ext
                    };
                    await db.UploadedFiles.AddAsync(up);
                }
                else
                {
                    TempData["msg"] = $"فرمت فایل {img.FileName} مجاز نیست. |danger";
                }
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                return Json(false);
            }
            return Json(true);
        }

        #endregion

        #region Config

        /*[HttpGet]
        public async Task<IActionResult> Config()
        {
            return View(await db.Config.AsNoTracking().FirstAsync());
        }

        [HttpPost]
        public async Task<JsonResult> Config(Config c)
        {
            string status;
            string msg;
            if (ModelState.IsValid)
            {
                db.Update(c);
                try
                {
                    await db.SaveChangesAsync();
                    msg = "عملیات موفقیت آمیز بود.";
                    status = "success";
                }
                catch (DbUpdateException e)
                {
                    msg = "عملیات با خطا مواجه شد. جزئیات: " + e.Message;
                    status = "danger";
                }
            }
            else
            {
                msg = "عملیات با خطا مواجه شد. جزئیات: خطای اعتبار سنجی فرم رخ داده است؛ لطفا فرم را بررسی کنید.";
                status = "danger";
            }
            return Json(new { msg, status });
        }*/

        #endregion

        #region Team

        [HttpGet]
        public async Task<IActionResult> Team(int? pageNumber)
        {
            var teams = db.Team;
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await teams.ToPagedListAsync(pageNum, 10);
            ViewBag.data = onePageOfData;

            return View();
        }

        [HttpGet]
        public IActionResult CreateTeam()
        {
            return PartialView("~/Views/Admin/Create/Team.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam(Team team, IFormFile avatarImage)
        {
            if (ModelState.IsValid)
            {
                if (avatarImage != null)
                {
                    if (avatarImage.Length <= 512000)//TODO Change size file
                    {
                        if (avatarImage.ContentType == "image/jpeg" || avatarImage.ContentType == "image/png")
                        {
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(avatarImage.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\team", fileName);
                            await using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await avatarImage.CopyToAsync(stream);
                            }

                            #region Resize Images to md & sm

                            using (Image img = Image.FromStream(avatarImage.OpenReadStream()))
                            {
                                var x = img.Resize(375, 250, true);
                                var pathMd = Path.Combine(_environment.WebRootPath, "img\\team", "md-" + fileName);
                                x.Save(pathMd);
                            }

                            using (Image img = Image.FromStream(avatarImage.OpenReadStream()))
                            {
                                var x = img.Resize(90, 60, true);
                                var pathSm = Path.Combine(_environment.WebRootPath, "img\\team", "sm-" + fileName);
                                x.Save(pathSm);
                            }

                            #endregion

                            team.Avatar = fileName;
                            await db.Team.AddAsync(team);
                            try
                            {
                                await db.SaveChangesAsync();
                                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 512 کیلوبایت است |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "لطفا یک تصویر انتخاب کنید. |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("Team");
        }

        [HttpGet]
        public async Task<IActionResult> EditTeam(int? id)
        {
            if (!id.HasValue) return NotFound();

            var team = await db.Team.FindAsync(id.Value);

            return PartialView("~/Views/Admin/Edit/Team.cshtml", team);
        }

        [HttpPost]
        public async Task<IActionResult> EditTeam(Team team, IFormFile avatarImage)
        {
            if (ModelState.IsValid)
            {
                if (avatarImage != null)
                {
                    if (avatarImage.Length <= 512000)//TODO Change size file
                    {
                        if (avatarImage.ContentType == "image/jpeg" || avatarImage.ContentType == "image/png")
                        {
                            var oldPic = Path.Combine(_environment.WebRootPath, "img\\team", team.Avatar);

                            string name = Path.GetRandomFileName();
                            string ext = Path.GetExtension(avatarImage.FileName).ToLower();
                            var fileName = name + ext;
                            var path = Path.Combine(_environment.WebRootPath, "img\\team", fileName);
                            await using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await avatarImage.CopyToAsync(stream);
                            }

                            #region Resize Images to md & sm

                            using (Image img = Image.FromStream(avatarImage.OpenReadStream()))
                            {
                                var x = img.Resize(375, 250, true);
                                var pathMd = Path.Combine(_environment.WebRootPath, "img\\team", "md-" + fileName);
                                x.Save(pathMd);
                            }

                            using (Image img = Image.FromStream(avatarImage.OpenReadStream()))
                            {
                                var x = img.Resize(90, 60, true);
                                var pathSm = Path.Combine(_environment.WebRootPath, "img\\team", "sm-" + fileName);
                                x.Save(pathSm);
                            }

                            #endregion

                            team.Avatar = fileName;
                            db.Update(team);
                            try
                            {
                                await db.SaveChangesAsync();
                                if (System.IO.File.Exists(oldPic))
                                {
                                    try
                                    {
                                        System.IO.File.Delete(oldPic);
                                    }
                                    catch (Exception e)
                                    {
                                        TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                                        return RedirectToAction("Team");
                                    }
                                }
                                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 512 کیلوبایت است |danger";
                    }
                }
                else
                {
                    db.Update(team);
                    try
                    {
                        await db.SaveChangesAsync();
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    catch (Exception e)
                    {
                        TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                    }
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("Team");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await db.Team.FindAsync(id);
            if (team != null)
            {
                var path = Path.Combine(_environment.WebRootPath, "img\\team", team.Avatar);
                var pathMd = Path.Combine(_environment.WebRootPath, "img\\team", "md-" + team.Avatar);
                var pathSm = Path.Combine(_environment.WebRootPath, "img\\team", "sm-" + team.Avatar);

                db.Team.Remove(team);
                try
                {
                    System.IO.File.Delete(path);
                    System.IO.File.Delete(pathMd);
                    System.IO.File.Delete(pathSm);

                    await db.SaveChangesAsync();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        #endregion

        #region Services

        [HttpGet]
        public async Task<IActionResult> ServiceSamples(int serviceId)//TODO Handle this sections
        {
            var workSamples = await db.WorkSamples.Where(w => w.ServiceId.Equals(serviceId)).ToListAsync();
            return RedirectToAction("Assist", new { workSamples = workSamples });
        }

        [HttpGet]
        public async Task<IActionResult> Service(int? pageNumber, int? serviceId /*List<WorkSample>? workSamples*/)
        {
            ViewData["serviceWorkSamples"] = await db.WorkSamples.Where(a=>a.ServiceId.Equals(serviceId)).ToListAsync();//TODO Design this section (front)
            /*ViewData["workSamples"] = workSamples;*/
            var services = db.Services;
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await services.ToPagedListAsync(pageNum, 10);
            ViewBag.data = onePageOfData;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateService()
        {
            ViewData["serviceId"] = await db.Services.ToListAsync();
            return View("~/Views/Admin/Create/Service.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateService(Service service, IFormFile servicePicture)
        {
            if (service.ParentId.Equals(0)) service.ParentId = null;

            if (ModelState.IsValid)
            {
                if (servicePicture != null)
                {
                    if (servicePicture.Length <= 512000)//TODO Change size file
                    {
                        if (servicePicture.ContentType == "image/jpeg" || servicePicture.ContentType == "image/png")
                        {
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(servicePicture.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\service", fileName);
                            await using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await servicePicture.CopyToAsync(stream);
                            }

                            using (Image img = Image.FromStream(servicePicture.OpenReadStream()))
                            {
                                var x = img.Resize(375, 250, true);
                                var pathMd = Path.Combine(_environment.WebRootPath, "img\\service", "md-" + fileName);
                                x.Save(pathMd);
                            }

                            using (Image img = Image.FromStream(servicePicture.OpenReadStream()))
                            {
                                var x = img.Resize(90, 60, true);
                                var pathSm = Path.Combine(_environment.WebRootPath, "img\\service", "sm-" + fileName);
                                x.Save(pathSm);
                            }

                            service.ServicePicture = fileName;
                            await db.Services.AddAsync(service);
                            try
                            {
                                await db.SaveChangesAsync();
                                #region Create Shortlink

                                string shortLink = _linkTools.GenerateShortLink(5, ShortLinkType.Service);
                                await db.ShortLink.AddAsync(new ShortLink
                                {
                                    ItemId = service.ServiceId,
                                    ShortKey = shortLink,
                                    Type = (int)ShortLinkType.Service
                                });
                                await db.SaveChangesAsync();

                                #endregion
                                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 512 کیلوبایت است |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "لطفا یک تصویر انتخاب کنید. |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("Service");
        }

        [HttpGet]
        public async Task<IActionResult> EditService(int? id)
        {
            if (!id.HasValue) return NotFound();

            ViewData["serviceId"] = await db.Services.ToListAsync();
            var service = await db.Services.FindAsync(id.Value);

            return View("~/Views/Admin/Edit/Service.cshtml", service);
        }

        [HttpPost]
        public async Task<IActionResult> EditService(Service service, IFormFile servicePicture)
        {
            if (service.ParentId.Equals(0)) service.ParentId = null;

            if (ModelState.IsValid)
            {
                if (servicePicture != null)
                {
                    if (servicePicture.Length <= 512000)//TODO Change size file
                    {
                        if (servicePicture.ContentType == "image/jpeg" || servicePicture.ContentType == "image/png")
                        {
                            var oldPic = Path.Combine(_environment.WebRootPath, "img\\service", service.ServicePicture);

                            string name = Path.GetRandomFileName();
                            string ext = Path.GetExtension(servicePicture.FileName).ToLower();
                            var fileName = name + ext;
                            var path = Path.Combine(_environment.WebRootPath, "img\\service", fileName);
                            await using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await servicePicture.CopyToAsync(stream);
                            }

                            #region Resize Images to md & sm

                            using (Image img = Image.FromStream(servicePicture.OpenReadStream()))
                            {
                                var x = img.Resize(375, 250, true);
                                var pathMd = Path.Combine(_environment.WebRootPath, "img\\service", "md-" + fileName);
                                x.Save(pathMd);
                            }

                            using (Image img = Image.FromStream(servicePicture.OpenReadStream()))
                            {
                                var x = img.Resize(90, 60, true);
                                var pathSm = Path.Combine(_environment.WebRootPath, "img\\service", "sm-" + fileName);
                                x.Save(pathSm);
                            }

                            #endregion

                            service.ServicePicture = fileName;
                            db.Services.Update(service);
                            try
                            {
                                await db.SaveChangesAsync();
                                if (System.IO.File.Exists(oldPic))
                                {
                                    try
                                    {
                                        System.IO.File.Delete(oldPic);
                                    }
                                    catch (Exception e)
                                    {
                                        TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                                        return RedirectToAction("Service");
                                    }
                                }
                                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 512 کیلوبایت است |danger";
                    }
                }
                else
                {
                    db.Services.Update(service);
                    try
                    {
                        await db.SaveChangesAsync();
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    catch (Exception e)
                    {
                        TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                    }
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("Service");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await db.Services.FindAsync(id);
            if (service != null)
            {
                var path = Path.Combine(_environment.WebRootPath, "img\\team", service.ServicePicture);
                var pathMd = Path.Combine(_environment.WebRootPath, "img\\team", "md-" + service.ServicePicture);
                var pathSm = Path.Combine(_environment.WebRootPath, "img\\team", "sm-" + service.ServicePicture);

                db.Services.Remove(service);
                try
                {
                    System.IO.File.Delete(path);
                    System.IO.File.Delete(pathMd);
                    System.IO.File.Delete(pathSm);

                    await db.SaveChangesAsync();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        #endregion

        #region WorkSample

        [HttpPost]
        public async Task<JsonResult> CheckDuplicateTitle(string workSampleTitle)
        {
            return Json(await db.WorkSamples.AnyAsync(w => w.Title.Equals(workSampleTitle)));
        }

        [HttpGet]
        public async Task<IActionResult> WorkSample(int? pageNumber)
        {
            var workSample = db.WorkSamples;
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await workSample.ToPagedListAsync(pageNum, 20);
            ViewBag.data = onePageOfData;

            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> CreateWorkSample(int? serviceId)//TODO Handle this sections 
        {
            var services = await db.Services.ToListAsync();
            if (serviceId.HasValue) ViewData["selectedCreateService"] = db.Services.FindAsync(serviceId.Value).Result.ServiceId;
            ViewData["WorkSampleServiceId"] = services;

            return View("~/Views/Admin/Create/WorkSample.cshtml");
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateWorkSample(WorkSample workSample, IFormFile sampleFile, int sampType)//, int? serviceId
        {
            if (ModelState.IsValid)
            {
                if (sampType.Equals(-1)) return RedirectToAction("CreateWorkSample", new { serviceId = workSample.ServiceId });
                if (workSample.ServiceId.Equals(0)) return RedirectToAction("CreateWorkSample", new { serviceId = workSample.ServiceId });

                if (await db.WorkSamples.AnyAsync(w => w.Title.Equals(workSample.Title)))
                {
                    TempData["msg"] = "عنوان نمونه کار نمیتواند تکراری باشد. |danger";
                    return RedirectToAction("CreateWorkSample", new { serviceId = workSample.ServiceId });
                }
                if (workSample.IsShow)
                {
                    if (await db.WorkSamples.Where(w => w.IsShow).CountAsync() >= 3)
                    {
                        TempData["msg"] = "برای نمایش نمونه‌کار در خدمات بیشتراز 3نمونه‌کار مجاز نیستید |danger";
                        return RedirectToAction("CreateWorkSample", new {serviceId = workSample.ServiceId});
                    }
                }

                /*if (Status.Equals(1)) workSample.Status = true;
                else workSample.Status = false;*/

                if (sampleFile != null)
                {
                    if ((sampleFile.ContentType == "image/jpeg" || sampleFile.ContentType == "image/png") && sampleFile.IsImage())
                    {
                        if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "worksample\\")))
                        {
                            Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "worksample\\"));
                        }

                        var fileName = Path.GetRandomFileName() + Path.GetExtension(sampleFile.FileName).ToLower();
                        var path = Path.Combine(_environment.WebRootPath, "worksample\\", $"pic-" + fileName);
                        await using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await sampleFile.CopyToAsync(stream);
                        }

                        #region Resize Images to md & sm

                        using (Image img = Image.FromStream(sampleFile.OpenReadStream()))
                        {
                            var x = img.Resize(375, 250, true);
                            var pathMd = Path.Combine(_environment.WebRootPath, "workSample\\", $"md-" + fileName);
                            try
                            {
                                x.Save(pathMd);
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات ریسایز عکس با مشکل مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }

                        using (Image img = Image.FromStream(sampleFile.OpenReadStream()))
                        {
                            var x = img.Resize(90, 60, true);
                            var pathSm = Path.Combine(_environment.WebRootPath, "workSample\\", $"sm-" + fileName);
                            try
                            {
                                x.Save(pathSm);
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات ریسایز عکس با مشکل مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }

                        #endregion

                        workSample.DocumentFile = fileName;
                        await db.WorkSamples.AddAsync(workSample);
                        try
                        {
                            await db.SaveChangesAsync();

                            #region Create Shortlink

                            string shortLink = _linkTools.GenerateShortLink(5, ShortLinkType.WorkSample);
                            await db.ShortLink.AddAsync(new ShortLink
                            {
                                ItemId = workSample.WorkSampleId,
                                ShortKey = shortLink,
                                Type = (int)ShortLinkType.WorkSample
                            });
                            await db.SaveChangesAsync();

                            #endregion

                            TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                        }
                        catch (Exception e)
                        {
                            TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "لطفا یک تصویر انتخاب کنید. |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("WorkSample");
        }

        [HttpGet]
        public async Task<IActionResult> EditWorkSample(int? workSampleId)//TODO Handle This Section
        {
            if (!workSampleId.HasValue) return NotFound();

            var workSample = await db.WorkSamples.FindAsync(workSampleId.Value);
            ViewData["currentService"] = workSample.ServiceId;
            ViewData["oldDocument"] = workSample.DocumentFile;
            ViewData["WorkSampleServiceId"] = await db.Services.ToListAsync();

            return PartialView("~/Views/Admin/Edit/WorkSample.cshtml", workSample);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditWorkSample(WorkSample workSample, IFormFile sampleFile, int sampType)//TODO Handle This Section
        {
            if (workSample.IsShow)
            {
                if (sampType.Equals(-1)) return View("Edit/WorkSample", workSample);
                if (workSample.ServiceId.Equals(0)) return View("Edit/WorkSample", workSample);

                if (await db.WorkSamples.Where(w => w.IsShow).CountAsync() >= 3)
                {
                    TempData["msg"] = "برای نمایش نمونه‌کار در خدمات بیشتراز 3تا مجاز نیستید |danger";
                    return RedirectToAction("EditWorkSample", new { serviceId = workSample.ServiceId });
                }
            }

            /*if (Status.Equals(1)) workSample.Status = true;
            else if (Status.Equals(0)) workSample.Status = false;*/

            if (ModelState.IsValid)
            {
                if (sampleFile != null)
                {
                    if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "worksample\\")))
                    {
                        Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "worksample\\"));
                    }

                    
                    if ((sampleFile.ContentType == "image/jpeg" || sampleFile.ContentType == "image/png") && sampleFile.IsImage())
                    {
                        #region Delete Old ImageFile

                        if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "worksample\\")))
                        {
                            Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "worksample\\"));
                        }
                        var deletePath = Path.Combine(_environment.WebRootPath, "workSample\\", $"pic-" + workSample.DocumentFile);
                        System.IO.File.Delete(deletePath);

                        #endregion

                        var fileName = Path.GetRandomFileName() + Path.GetExtension(sampleFile.FileName).ToLower();
                        var path = Path.Combine(_environment.WebRootPath, "worksample\\", $"pic-" + fileName);
                        await using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await sampleFile.CopyToAsync(stream);
                        }

                        #region Resize Images to md & sm

                        using (Image img = Image.FromStream(sampleFile.OpenReadStream()))
                        {
                            var x = img.Resize(375, 250, true);
                            var pathMd = Path.Combine(_environment.WebRootPath, "worksample\\", "md-" + fileName);
                            try
                            {
                                x.Save(pathMd);
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات ریسایز عکس با مشکل مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }

                        using (Image img = Image.FromStream(sampleFile.OpenReadStream()))
                        {
                            var x = img.Resize(90, 60, true);
                            var pathSm = Path.Combine(_environment.WebRootPath, "worksample\\", "sm-" + fileName);
                            try
                            {
                                x.Save(pathSm);
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات ریسایز عکس با مشکل مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }

                        #endregion

                        workSample.DocumentFile = fileName;
                        db.WorkSamples.Update(workSample);
                        try
                        {
                            await db.SaveChangesAsync();
                            TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                        }
                        catch (Exception e)
                        {
                            TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                    }
                }
                else
                {
                    db.Update(workSample);
                    try
                    {
                        await db.SaveChangesAsync();
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    catch (Exception e)
                    {
                        TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                    }
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("WorkSample");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWorkSample(int id, int? workSampleId)
        {
            if (workSampleId.HasValue)
            {
                var workSampleFile = await db.WorkSamples.FindAsync(workSampleId);
                if (workSampleFile != null)
                {
                    var pathImage = Path.Combine(_environment.WebRootPath, "img\\workSample\\" + workSampleFile.Title, $"pic-" + workSampleFile.DocumentFile);
                    var pathMd = Path.Combine(_environment.WebRootPath, "img\\workSample\\" + workSampleFile.Title, $"md-" + workSampleFile.DocumentFile);
                    var pathSm = Path.Combine(_environment.WebRootPath, "img\\workSample\\" + workSampleFile.Title, $"sm-" + workSampleFile.DocumentFile);
                    workSampleFile.DocumentFile = "";
                    db.WorkSamples.Update(workSampleFile);
                    try
                    {
                        System.IO.File.Delete(pathImage);
                        System.IO.File.Delete(pathMd);
                        System.IO.File.Delete(pathSm);
                        await db.SaveChangesAsync();
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    catch (Exception e)
                    {
                        TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                    }
                }
            }

            var workSample = await db.WorkSamples.FindAsync(id);
            if (workSample != null)
            {
                var pathImage = Path.Combine(_environment.WebRootPath, "worksample\\", $"pic-" + workSample.DocumentFile);
                var pathMd = Path.Combine(_environment.WebRootPath, "worksample\\", $"md-" + workSample.DocumentFile);
                var pathSm = Path.Combine(_environment.WebRootPath, "worksample\\", $"sm-" + workSample.DocumentFile);
                db.WorkSamples.Remove(workSample);
                try
                {
                    System.IO.File.Delete(pathImage);
                    System.IO.File.Delete(pathMd);
                    System.IO.File.Delete(pathSm);
                    if (Directory.Exists(Path.Combine(_environment.WebRootPath, "worksample\\")))
                    {
                        Directory.Delete(Path.Combine(_environment.WebRootPath, "worksample\\"));
                    }
                    await db.SaveChangesAsync();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<JsonResult> WorkSampleDocumentFile(int? workSampleId, IFormFile sampleFile)
        {
            if (sampleFile == null) return Json(false);
            var workSample = await db.WorkSamples.FindAsync(workSampleId);
            if (workSample != null)
            {
                if ((sampleFile.ContentType == "image/jpeg" || sampleFile.ContentType == "image/png") && sampleFile.IsImage())
                {
                    if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "worksample\\")))
                    {
                        Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "worksample\\"));
                    }
                    else
                    {
                        var deletePath = Path.Combine(_environment.WebRootPath, "worksample\\", $"pic-" + workSample.DocumentFile);
                        System.IO.File.Delete(deletePath);
                    }

                    var fileName = Path.GetRandomFileName() + Path.GetExtension(sampleFile.FileName).ToLower();
                    var path = Path.Combine(_environment.WebRootPath, "worksample\\", $"pic-" + fileName);
                    await using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await sampleFile.CopyToAsync(stream);
                    }

                    #region Resize Images to md & sm

                    using (Image img = Image.FromStream(sampleFile.OpenReadStream()))
                    {
                        var x = img.Resize(375, 250, true);
                        var pathMd = Path.Combine(_environment.WebRootPath, "worksample\\", "md-" + fileName);
                        try
                        {
                            x.Save(pathMd);
                        }
                        catch (Exception e)
                        {
                            TempData["msg"] = $"عملیات ریسایز عکس با مشکل مواجه شد. جزئیات: {e.Message} |danger";
                        }
                    }

                    using (Image img = Image.FromStream(sampleFile.OpenReadStream()))
                    {
                        var x = img.Resize(90, 60, true);
                        var pathSm = Path.Combine(_environment.WebRootPath, "worksample\\", "sm-" + fileName);
                        try
                        {
                            x.Save(pathSm);
                        }
                        catch (Exception e)
                        {
                            TempData["msg"] = $"عملیات ریسایز عکس با مشکل مواجه شد. جزئیات: {e.Message} |danger";
                        }
                    }

                    #endregion

                    workSample.DocumentFile = fileName;
                    db.WorkSamples.Update(workSample);
                    try
                    {
                        await db.SaveChangesAsync();
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    catch (Exception e)
                    {
                        TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                    return Json(false);
                }
            }

            return Json(true);
        }
        
        [HttpPost]
        public async Task<IActionResult> DownloadWorkSample(int workSampleId)
        {
            var workSampleDownload = await db.WorkSamples.FindAsync(workSampleId);

            #region download File

            string downloadFilePath = Path.Combine(_environment.WebRootPath, "worksample\\", $"pic-" + workSampleDownload.DocumentFile);
            string downloadFileName = workSampleDownload.DocumentFile;

            byte[] downloadFile = await System.IO.File.ReadAllBytesAsync(downloadFilePath);
            return File(downloadFile, "application/force-download", downloadFileName);

            #endregion
        }

        #endregion

        #region Faq

        [HttpGet]
        public async Task<IActionResult> Faq(int? pageNumber)
        {
            var data = db.Faqs.OrderByDescending(a => a.Sort);
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await data.ToPagedListAsync(pageNum, 10);
            ViewBag.data = onePageOfData;

            ViewData["tags"] = await db.Tag.Where(a => a.Type.Equals((byte)TagType.Faq)).ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateFaq()
        {
            ViewData["tags"] = await db.Tag.Where(a => a.Type.Equals((byte)TagType.Faq)).ToListAsync();
            return View("~/Views/Admin/Create/Faq.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaq(Faq faq, int[] tags)
        {
            if (ModelState.IsValid)
            {
                await db.Faqs.AddAsync(faq);
                try
                {
                    await db.SaveChangesAsync();
                    if (tags.Length > 0)
                    {
                        foreach (var item in tags)
                        {
                            db.Add(new FaqTag()
                            {
                                FaqId = faq.FaqId,
                                TagId = item
                            });
                        }
                    }

                    await db.SaveChangesAsync();

                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("Faq");
        }

        [HttpGet]
        public async Task<IActionResult> EditFaq(int? id)
        {
            if (!id.HasValue) return NotFound();

            var faq = await db.Faqs.FindAsync(id.Value);
            var faqCurrentTags = await db.FaqTags.AsNoTracking().Where(a => a.FaqId.Equals(id.Value)).Select(a => a.TagId).ToListAsync();
            ViewData["faqCurrentTags"] = await db.Tag.AsNoTracking().Where(a => faqCurrentTags.Contains(a.TagId)).Select(a => a.TagId).ToListAsync();
            ViewData["tags"] = await db.Tag.Where(a => a.Type.Equals((byte)TagType.Faq)).AsNoTracking().ToListAsync();

            return PartialView("~/Views/Admin/Edit/Faq.cshtml", faq);
        }

        [HttpPost]
        public async Task<IActionResult> EditFaq(Faq faq, int[] tags)
        {
            if (ModelState.IsValid)
            {
                var currentTags = await db.FaqTags.Where(a => a.FaqId.Equals(faq.FaqId)).ToListAsync();
                if (currentTags.Count > 0) { db.FaqTags.RemoveRange(currentTags.AsEnumerable()); }

                if (tags.Length > 0)
                {
                    foreach (var item in tags)
                    {
                        db.Add(new FaqTag()
                        {
                            FaqId = faq.FaqId,
                            TagId = item
                        });
                    }
                }
                db.Faqs.Update(faq);
                try
                {
                    await db.SaveChangesAsync();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("Faq");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            var faq = await db.Faqs.FindAsync(id);
            if (faq != null)
            {
                db.Faqs.Remove(faq);
                try
                {
                    await db.SaveChangesAsync();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        #endregion

        //TODO Design FrontEnd(showAssist enother section)
        #region Assist

        [HttpGet]
        public async Task<IActionResult> Assist(int? pageNumber, int? assistId)
        {
            if (Request.IsAjaxRequest() && assistId.HasValue)
            {
                var assistInformation = await db.Assists.FindAsync(assistId);
                return PartialView("_AssistInformation", assistInformation);
            }
            var assists = db.Assists;
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await assists.ToPagedListAsync(pageNum, 10);
            ViewBag.data = onePageOfData;

            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Assist(string filterBy, string fullname, string phone, string socialId, string location, int? pageNumber)
        {
            var data = db.Assists.AsQueryable();
            switch (filterBy)
            {
                case "fullname":
                    if (!string.IsNullOrEmpty(fullname))
                    {
                        data = data.Where(a => a.FullName.Contains(fullname));
                    }
                    break;
                case "phone":
                    if (!string.IsNullOrEmpty(phone) && phone.Length <=11)
                    {
                        data = data.Where(a => a.PhoneNumber.Contains(phone));
                    }
                    break;
                case "socialId":
                    if (!string.IsNullOrEmpty(socialId))
                    {
                        data = data.Where(a => a.SocialId.Contains(socialId));
                    }
                    break;
                case "location":
                    if (!string.IsNullOrEmpty(location))
                    {
                        data = data.Where(a => a.Location.Contains(location));
                    }
                    break;
            }
            ViewBag.filterBy = filterBy;
            ViewBag.fullname = fullname;
            ViewBag.phone = phone;
            ViewBag.socialId = socialId;
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await data.OrderByDescending(a => a.AssistId).ToPagedListAsync(pageNum, 15);
            ViewBag.data = onePageOfData;
            return PartialView("_Assist");
        }

        [HttpPost]
        public async Task<IActionResult> DownloadCvFile(int assistId)
        {
            var cvFileDownload = await db.Assists.FindAsync(assistId);

            #region download File

            if (Directory.Exists(Path.Combine(_environment.WebRootPath, "assist\\cvFiles", $"cv-" + cvFileDownload.CvFileName)))
            {
                string downloadFilePath = Path.Combine(_environment.WebRootPath, "assist\\cvFiles", $"cv-" + cvFileDownload.CvFileName);
                string downloadFileName = cvFileDownload.CvFileName;

                if (!string.IsNullOrEmpty(downloadFilePath))
                {
                    byte[] downloadFile = await System.IO.File.ReadAllBytesAsync(downloadFilePath);
                    return File(downloadFile, "application/force-download", downloadFileName);
                }
            }

            #endregion

            return Json(false);
        }

        #endregion

        #region Tag

        [HttpGet]
        public async Task<IActionResult> Tag(int? pageNumber)
        {
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await db.Tag.AsNoTracking().OrderByDescending(a => a.TagId).ToPagedListAsync(pageNum, 10);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Tag(Tag t)
        {
            if (ModelState.IsValid)
            {
                if (await db.Tag.AnyAsync(a => a.UniqueName.Equals(t.UniqueName)))
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: مسیر {t.UniqueName} قبلا ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                await db.AddAsync(t);
                try
                {
                    await db.SaveChangesAsync();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<JsonResult> TagDuplicate(string value)
        {
            string msg = "";
            string status = "";
            if (string.IsNullOrWhiteSpace(value))
            {
                msg = "لطفا عنوان مسیر را تایپ نمائید.";
                status = "danger";
            }
            else
            {
                if (await db.Tag.AsNoTracking().AnyAsync(a => a.UniqueName.Equals(value)))
                {
                    msg = "این عنوان مسیر از قبل ثبت شده است.";
                    status = "danger";
                }
                else
                {
                    msg = "این عنوان مسیر از قبل ثبت نشده است و می توانید آن را ثبت نمائید.";
                    status = "success";
                }
            }
            return Json(new { msg, status });
        }

        [HttpGet]
        public async Task<PartialViewResult> EditTag(int id)
        {
            return PartialView("~/Views/Admin/Edit/Tag.cshtml", await db.Tag.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditTag(Tag t)
        {
            if (ModelState.IsValid)
            {
                if (await db.Tag.AnyAsync(a => a.UniqueName.Equals(t.UniqueName) && !a.TagId.Equals(t.TagId)))
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: مسیر {t.UniqueName} قبلا ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var tag = await db.Tag.FindAsync(t.TagId);
                tag.Name = t.Name;
                tag.UniqueName = t.UniqueName;
                tag.Type = t.Type;

                db.Update(tag);
                try
                {
                    await db.SaveChangesAsync();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var t = await db.Tag.FindAsync(id);
            db.Remove(t);
            try
            {
                db.SaveChanges();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (DbUpdateException e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return RedirectToAction("Tag");
        }

        public async Task<IActionResult> TagSearch(string title, int? pageNumber)
        {
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await db.Tag.Where(a => a.Name.Contains(title) || a.UniqueName.Contains(title)).ToPagedListAsync(pageNum, 10);
            ViewBag.data = onePageOfData;
            return PartialView("_TagSearch");
        }

        #endregion

        //TODO Check and Fix Blog
        #region Blog

        /*[HttpGet]
        public async Task<IActionResult> Blog(int? pageNumber)
        {
            var data = db.Blog.OrderByDescending(a => a.CreateDate);
            var pageNum = pageNumber ?? 1;
            var onePageOfData = await data.ToPagedListAsync(pageNum, 10);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateBlog()
        {
            ViewData["tags"] = await db.Tag.Where(a => a.Type.Equals((byte)TagType.Blog)).AsNoTracking().ToListAsync();
            return View("~/Views/Admin/Create/Blog.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog(Blog blog, string rawDate, int[] tags, IFormFile mobilePicture, IFormFile deskPicture)
        {
            if (ModelState.IsValid)
            {
                if (deskPicture != null)
                {
                    if (deskPicture.Length <= 512000)
                    {
                        if (deskPicture.ContentType == "image/jpeg" || deskPicture.ContentType == "image/png")
                        {
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(deskPicture.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\blog", fileName);
                            await using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await deskPicture.CopyToAsync(stream);
                            }

                            using (Image img = Image.FromStream(deskPicture.OpenReadStream()))
                            {
                                var x = img.Resize(375, 250, true);
                                var pathSm = Path.Combine(_environment.WebRootPath, "img\\blog", "md-" + fileName);
                                x.Save(pathSm);
                            }

                            using (Image img = Image.FromStream(deskPicture.OpenReadStream()))
                            {
                                var x = img.Resize(90, 60, true);
                                var pathSm = Path.Combine(_environment.WebRootPath, "img\\blog", "sm-" + fileName);
                                x.Save(pathSm);
                            }

                            blog.DeskImage = fileName;

                            var dateTime = rawDate.Split(' ');
                            var date = dateTime[0];
                            var time = dateTime[1];
                            var Date = date.PersianToEnglish().Split('/');
                            var Time = time.PersianToEnglish().Split(':');
                            blog.CreateDate = new PersianDateTime(Convert.ToInt32(Date[0]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[2]), Convert.ToInt32(Time[0]), Convert.ToInt32(Time[1]), Convert.ToInt32(Time[2])).ToDateTime();
                            blog.ViewCount = 0;
                            blog.ApplicationUserId = _userManager.GetUserId(User);
                            await db.Blog.AddAsync(blog);
                            try
                            {
                                await db.SaveChangesAsync();

                                string shortLink = _linkTools.GenerateShortLink(5, ShortLinkType.Blog);
                                await db.ShortLink.AddAsync(new ShortLink
                                {
                                    ItemId = blog.BlogId,
                                    ShortKey = shortLink,
                                    Type = (int)ShortLinkType.Blog
                                });

                                if (tags.Length > 0)
                                {
                                    foreach (var item in tags)
                                    {
                                        db.Add(new BlogTag
                                        {
                                            BlogId = blog.BlogId,
                                            TagId = item
                                        });
                                    }
                                }

                                await db.SaveChangesAsync();

                                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 512 کیلوبایت است |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "لطفا یک تصویر انتخاب کنید. |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("Blog");
        }

        [HttpGet]
        public async Task<IActionResult> EditBlog(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var item = await db.Blog.FindAsync(id.Value);
            var currentTags = await db.BlogTag.AsNoTracking().Where(a => a.BlogId.Equals(id.Value)).Select(a => a.TagId).ToListAsync();
            ViewData["currentTags"] = await db.Tag.AsNoTracking().Where(a => currentTags.Contains(a.TagId)).Select(a => a.TagId).ToListAsync();
            ViewData["tags"] = await db.Tag.Where(a => a.Type.Equals((byte)TagType.Blog)).AsNoTracking().ToListAsync();
            return View("~/Views/Admin/Edit/Blog.cshtml", item);
        }

        [HttpPost]
        public async Task<IActionResult> EditBlog(Blog blog, string rawDate, int[] tags, IFormFile mobilePicture, IFormFile deskPicture)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(rawDate))
                {
                    var dateTime = rawDate.Split(' ');
                    var date = dateTime[0];
                    var time = dateTime[1];
                    var Date = date.PersianToEnglish().Split('/');
                    var Time = time.PersianToEnglish().Split(':');
                    blog.CreateDate = new PersianDateTime(Convert.ToInt32(Date[0]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[2]), Convert.ToInt32(Time[0]), Convert.ToInt32(Time[1]), Convert.ToInt32(Time[2])).ToDateTime();
                }

                var currentTags = await db.BlogTag.Where(a => a.BlogId.Equals(blog.BlogId)).ToListAsync();
                if (currentTags.Count > 0)
                {
                    db.BlogTag.RemoveRange(currentTags.AsEnumerable());
                }
                if (tags.Length > 0)
                {
                    foreach (var item in tags)
                    {
                        db.Add(new BlogTag
                        {
                            BlogId = blog.BlogId,
                            TagId = item
                        });
                    }
                }
                if (deskPicture != null)
                {
                    if (deskPicture.Length <= 512000)
                    {
                        if (deskPicture.ContentType == "image/jpeg" || deskPicture.ContentType == "image/png")
                        {
                            var oldPic = Path.Combine(_environment.WebRootPath, "img\\blog", blog.DeskImage);

                            string name = Path.GetRandomFileName();
                            string ext = Path.GetExtension(deskPicture.FileName).ToLower();
                            var fileName = name + ext;
                            var path = Path.Combine(_environment.WebRootPath, "img\\blog", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await deskPicture.CopyToAsync(stream);
                            }

                            using (Image img = Image.FromStream(deskPicture.OpenReadStream()))
                            {
                                var x = img.Resize(375, 250, true);
                                var pathSm = Path.Combine(_environment.WebRootPath, "img\\blog", "md-" + fileName);
                                x.Save(pathSm);
                            }

                            using (Image img = Image.FromStream(deskPicture.OpenReadStream()))
                            {
                                var x = img.Resize(90, 60, true);
                                var pathSm = Path.Combine(_environment.WebRootPath, "img\\blog", "sm-" + fileName);
                                x.Save(pathSm);
                            }

                            blog.DeskImage = fileName;
                            db.Update(blog);
                            try
                            {
                                await db.SaveChangesAsync();
                                if (System.IO.File.Exists(oldPic))
                                {
                                    try
                                    {
                                        System.IO.File.Delete(oldPic);
                                    }
                                    catch (Exception e)
                                    {
                                        TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                                        return RedirectToAction("Blog");
                                    }
                                }
                                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 512 کیلوبایت است |danger";
                    }
                }
                else
                {
                    db.Update(blog);
                    try
                    {
                        await db.SaveChangesAsync();
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    catch (Exception e)
                    {
                        TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                    }
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("Blog");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await db.Blog.FindAsync(id);
            if (blog != null)
            {
                var path = Path.Combine(_environment.WebRootPath, "img\\blog", blog.DeskImage);
                var pathMd = Path.Combine(_environment.WebRootPath, "img\\blog", "md-" + blog.DeskImage);
                var pathSm = Path.Combine(_environment.WebRootPath, "img\\blog", "sm-" + blog.DeskImage);

                db.Blog.Remove(blog);
                try
                {
                    System.IO.File.Delete(path);
                    System.IO.File.Delete(pathMd);
                    System.IO.File.Delete(pathSm);

                    await db.SaveChangesAsync();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }*/

        #endregion

        #region User

        /*[HttpPost]
        public async Task<ActionResult> UserLock(string id, string note)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(note))
                    {
                        user.LockoutReason = note;
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (!updateResult.Succeeded)
                        {
                            TempData["msg"] = "عملیات با خطا مواجه شد. (update note) |danger";
                            return Redirect(Request.Headers["Referer"].ToString());
                        }
                    }
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    var result = await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(10));
                    if (result.Succeeded)
                    {
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    else
                    {
                        TempData["msg"] = "عملیات با خطا مواجه شد. |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "کاربر پیدا نشد. |danger";
                }
            }
            else
            {
                TempData["msg"] = "درخواست غیر مجاز. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<ActionResult> UserUnlock(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.LockoutReason = "";
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        TempData["msg"] = "عملیات با خطا مواجه شد. (update note) |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                    var result = await _userManager.SetLockoutEndDateAsync(user, null);
                    if (result.Succeeded)
                    {
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    else
                    {
                        TempData["msg"] = "عملیات با خطا مواجه شد. |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "کاربر پیدا نشد. |danger";
                }
            }
            else
            {
                TempData["msg"] = "درخواست غیر مجاز. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<JsonResult> EditUserRole(string userId, string roleName, bool Value)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (Value)
            {
                var result = await _userManager.AddToRoleAsync(appUser, roleName);
                if (result.Succeeded)
                    return Json(true);
                else
                    return Json(false);

            }
            else
            {
                var result = await _userManager.RemoveFromRoleAsync(appUser, roleName);
                if (result.Succeeded)
                    return Json(true);
                else
                    return Json(false);
            }
        }

        public async Task<IActionResult> Users(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _userManager.Users.OrderByDescending(a => a.RegisterDate).ToPagedListAsync(pageNumber, 10);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            ViewData["userRoles"] = await _userManager.GetRolesAsync(user);
            ViewData["roles"] = await _roleManager.Roles.OrderBy(a => a.Name).ToListAsync();

            return View("~/Views/Admin/Edit/User.cshtml", new EditUserVM
            {
                Avatar = user.Avatar,
                Birth = user.Birth,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Firstname = user.Firstname,
                Id = user.Id,
                Lastname = user.Lastname,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                NationalId = user.NationalId,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Tel = user.Tel
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserVM u, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                u.NationalId = u.NationalId.PersianToEnglish();
                if (!u.NationalId.isNumber())
                {
                    TempData["msg"] = "کد ملی وارد شده باید یک عدد باشد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (await _userManager.Users.AnyAsync(a => a.NationalId.Equals(u.NationalId) && !a.Id.Equals(u.Id)))
                {
                    TempData["msg"] = "کد ملی وارد شده در سیستم وجود دارد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                u.PhoneNumber = u.PhoneNumber.PersianToEnglish();
                if (!u.PhoneNumber.isNumber())
                {
                    TempData["msg"] = "شماره تلفن همراه وارد شده باید یک عدد باشد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (await _userManager.Users.AnyAsync(a => a.PhoneNumber.Equals(u.PhoneNumber) && !a.Id.Equals(u.Id)))
                {
                    TempData["msg"] = "شماره تلفن همراه وارد شده در سیستم ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var user = await _userManager.FindByIdAsync(u.Id);

                string[] b = u.BirthString.PersianToEnglish().Split('/');
                user.Birth = new PersianDateTime(Convert.ToInt32(b[0]), Convert.ToInt32(b[1]), Convert.ToInt32(b[2])).ToDateTime();
                user.Firstname = u.Firstname;
                user.Lastname = u.Lastname;
                user.Email = u.Email;
                user.EmailConfirmed = u.EmailConfirmed;
                user.NationalId = u.NationalId;
                user.Tel = u.Tel;
                user.PhoneNumber = u.PhoneNumber;
                user.PhoneNumberConfirmed = u.PhoneNumberConfirmed;

                var validTypes = new string[] { "image/jpeg", "image/png" };
                if (pic != null)
                {
                    if (validTypes.Contains(pic.ContentType))
                    {
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                        using (var stream = new FileStream(Path.Combine(_environment.WebRootPath, "img\\user", fileName), FileMode.Create))
                        {
                            await pic.CopyToAsync(stream);
                        }
                        if (!string.IsNullOrEmpty(user.Avatar))
                        {
                            try
                            {
                                System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\user", user.Avatar));
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                                return Redirect(Request.Headers["Referer"].ToString());
                            }
                        }
                        user.Avatar = fileName;
                    }
                    else
                    {
                        TempData["msg"] = $"فرمت فایل های ارسالی مجاز نیست. باید png یا jpg ارسال شود. |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                }

                var update = await _userManager.UpdateAsync(user);
                if (update.Succeeded)
                {
                    TempData["msg"] = "عملیات موفقیت آمیز بود |success";
                    return RedirectToAction("Users");
                }
                else
                {
                    string error = "";
                    foreach (var item in update.Errors)
                    {
                        if (item.Equals(update.Errors.Last()))
                        {
                            error = error + item.Code + " " + item.Description;
                        }
                        else
                        {
                            error = error + item.Code + " " + item.Description + " | ";
                        }
                    }
                    TempData["msg"] = $"خطا در ذخیره اطلاعات و برقراری ارتباط با پایگاه داده رخ داده است. لطفا مجدد تلاش کنید. جزئیات: {error} |danger";
                }
            }
            else
            {
                TempData["msg"] = "خطای اعتبار سنجی رخ داده است. لطفا فرم را بررسی کنید |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }*/

        #endregion

        #region MainSlider

        /*[HttpGet]
        public async Task<IActionResult> Slider()
        {
            return View(await db.MainSlider.AsNoTracking().OrderBy(a => a.Sort).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Slider(MainSlider m, IFormFile picture, IFormFile pictureSm)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            if (picture == null)
            {
                TempData["msg"] = "لطفا یک تصویر انتخاب کنید |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            if (pictureSm == null)
            {
                TempData["msg"] = "لطفا یک تصویر موبایل انتخاب کنید |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            if (picture.Length > 512000)
            {
                TempData["msg"] = "آخرین حجم مجاز برای این تصویر 512 کیلوبایت است |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            if (pictureSm.Length > 512000)
            {
                TempData["msg"] = "آخرین حجم مجاز برای این تصویر موبایل 512 کیلوبایت است |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var validTypes = new string[] { "image/jpeg", "image/png", "image/webp" };

            if (!validTypes.Contains(picture.ContentType))
            {
                TempData["msg"] = "فرمت قایل ارسالی مجاز نیست. |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            if (!validTypes.Contains(pictureSm.ContentType))
            {
                TempData["msg"] = "فرمت فایل ارسالی برای موبایل مجاز نیست. |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }


            var fileName = Path.GetRandomFileName() + Path.GetExtension(picture.FileName).ToLower();
            var path = Path.Combine(_environment.WebRootPath, "img\\slider", fileName);

            var fileNameSm = Path.GetRandomFileName() + Path.GetExtension(pictureSm.FileName).ToLower();
            var pathSm = Path.Combine(_environment.WebRootPath, "img\\slider", fileNameSm);

            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\slider")))
            {
                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\slider"));
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await picture.CopyToAsync(stream);
            }
            using (var stream = new FileStream(pathSm, FileMode.Create))
            {
                await pictureSm.CopyToAsync(stream);
            }

            try
            {
                m.Image = fileName;
                m.ImageSm = fileNameSm;
                m.Date = DateTime.Now;
                if (!m.Expire.HasValue)
                {
                    m.Expire = 9999;
                }
                db.MainSlider.Add(m);
                await db.SaveChangesAsync();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return RedirectToAction("Slider");
        }

        [HttpPost]
        public async Task<JsonResult> EditSlider(MainSlider m, int? newExpire)
        {
            string msg;
            string stat;
            string newpd = "";
            if (ModelState.IsValid)
            {
                if (newExpire.HasValue)
                {
                    m.Expire = newExpire.Value;
                    m.Date = DateTime.Now;
                    newpd = new PersianDateTime(m.Date.AddDays(newExpire.Value)).ToString("yyyy/MM/dd");
                }
                db.Update(m);
                try
                {
                    await db.SaveChangesAsync();
                    msg = "تغییرات با موفقیت ذخیره شد";
                    stat = "success";
                }
                catch (DbUpdateException e)
                {
                    msg = "خطا در ذخیره اطلاعات و برقراری ارتباط با پایگاه داده رخ داده است. لطفا مجدد تلاش کنید. جزئیات: " + e.Message;
                    stat = "danger";
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    stat = "danger";
                }
            }
            else
            {
                msg = "خطا در اعتبار سنجی فرم وجود دارد. لطفا فرم را بررسی و سپس تلاش کنید.";
                stat = "danger";
            }
            return Json(new { message = msg, status = stat, newpd });
        }

        public async Task<JsonResult> RemoveSlide(int id)
        {
            string msg;
            string stat;
            var i = await db.MainSlider.FindAsync(id);
            if (i != null)
            {
                db.Remove(i);
                try
                {
                    await db.SaveChangesAsync();
                    msg = "عملیات حذف موفقیت آمیز بود.";
                    stat = "success";
                }
                catch (DbUpdateException e)
                {
                    msg = "خطا در ذخیره اطلاعات و برقراری ارتباط با پایگاه داده رخ داده است. لطفا مجدد تلاش کنید. جزئیات: " + e.Message;
                    stat = "danger";
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    stat = "danger";
                }
                //deleting from disk
                var path = Path.Combine(_environment.WebRootPath, "img\\slider", i.Image);
                var pathSm = Path.Combine(_environment.WebRootPath, "img\\slider", i.ImageSm);
                FileInfo fi = new FileInfo(path);
                FileInfo fiSm = new FileInfo(pathSm);
                try
                {
                    fi.Delete();
                    fiSm.Delete();
                }
                catch (IOException e)
                {
                    msg = e.Message;
                    stat = "danger";
                }
            }
            else
            {
                msg = "مورد درخواستی یافت نشد";
                stat = "danger";
            }
            return Json(new { message = msg, status = stat });
        }

        [HttpPost]
        public async Task<JsonResult> EditSliderImage(IFormFile image, int id)
        {
            string stat;
            string msg;
            if (image != null)
            {
                if (image.Length <= 512000)
                {
                    if (image.ContentType == "image/jpeg" || image.ContentType == "image/png" || image.ContentType == "image/webp")
                    {
                        var i = await db.MainSlider.FindAsync(id);

                        var path = Path.Combine(_environment.WebRootPath, "img\\slider", i.Image);
                        if (System.IO.File.Exists(path))
                        {
                            try
                            {
                                System.IO.File.Delete(path);
                            }
                            catch (IOException e)
                            {
                                msg = "خطا! فایل تصویری در حال استفاده در یک پردازش دیگر است. لطفا دوباره امتحان کنید. جزئیات: " + e.Message;
                                stat = "uk-alert-danger";
                                return Json(new { message = msg, status = stat });
                            }
                        }

                        var fileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName).ToLower();
                        var P = Path.Combine(_environment.WebRootPath, "img\\slider", fileName);
                        using (var stream = new FileStream(P, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        try
                        {
                            i.Image = fileName;
                            db.Update(i);
                            await db.SaveChangesAsync();
                            msg = "تصویر با موفقیت آپلود و جایگزین شد";
                            stat = "success";
                        }
                        catch (NotImplementedException notImp)
                        {
                            msg = notImp.Message;
                            stat = "danger";
                        }
                    }
                    else
                    {
                        msg = "لطفا از فرمت jpg  یا png استفاده کنید";
                        stat = "danger";
                    }
                }
                else
                {
                    msg = "حجم تصویر بیشتر از 512 کیلوبایت است";
                    stat = "danger";
                }
            }
            else
            {
                msg = "لطفا یک تصویر انتخاب کنید.";
                stat = "danger";
            }
            return Json(new
            {
                message = msg,
                status = stat
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditSliderImageSm(IFormFile image, int id)
        {
            string stat;
            string msg;
            if (image != null)
            {
                if (image.Length <= 512000)
                {
                    if (image.ContentType == "image/jpeg" || image.ContentType == "image/png" || image.ContentType == "image/webp")
                    {
                        var i = await db.MainSlider.FindAsync(id);

                        var path = Path.Combine(_environment.WebRootPath, "img\\slider", i.ImageSm);
                        if (System.IO.File.Exists(path))
                        {
                            try
                            {
                                System.IO.File.Delete(path);
                            }
                            catch (IOException e)
                            {
                                msg = "خطا! فایل تصویری در حال استفاده در یک پردازش دیگر است. لطفا دوباره امتحان کنید. جزئیات: " + e.Message;
                                stat = "uk-alert-danger";
                                return Json(new { message = msg, status = stat });
                            }
                        }

                        var fileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName).ToLower();
                        var P = Path.Combine(_environment.WebRootPath, "img\\slider", fileName);
                        using (var stream = new FileStream(P, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        try
                        {
                            i.ImageSm = fileName;
                            db.Update(i);
                            await db.SaveChangesAsync();
                            msg = "تصویر با موفقیت آپلود و جایگزین شد";
                            stat = "success";
                        }
                        catch (NotImplementedException notImp)
                        {
                            msg = notImp.Message;
                            stat = "danger";
                        }
                    }
                    else
                    {
                        msg = "لطفا از فرمت jpg  یا png استفاده کنید";
                        stat = "danger";
                    }
                }
                else
                {
                    msg = "حجم تصویر بیشتر از 512 کیلوبایت است";
                    stat = "danger";
                }
            }
            else
            {
                msg = "لطفا یک تصویر انتخاب کنید.";
                stat = "danger";
            }
            return Json(new
            {
                message = msg,
                status = stat
            });
        }*/

        #endregion

        #region FeedbackSlider

        [HttpGet]
        public async Task<IActionResult> FeedbackSlider()
        {
            return View(await db.UsersFeedbacks.AsNoTracking().OrderBy(a => a.Sort).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> FeedbackSlider(UsersFeedback m, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    if (image.Length <= 512000)
                    {
                        if (image.ContentType == "image/jpeg" || image.ContentType == "image/png" || image.ContentType == "image/webp")
                        {
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\slider", fileName);

                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\slider")))
                            {
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\slider"));
                            }

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }
                            try
                            {
                                m.Picture = fileName;
                                db.Add(m);
                                await db.SaveChangesAsync();
                                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                            }
                        }
                        else
                        {
                            TempData["msg"] = "لطفا یک فایل با پسوند jpg یا png انتخاب کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "آخرین حجم مجاز برای این تصویر 512 کیلوبایت است |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "لطفا یک تصویر انتخاب کنید |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return RedirectToAction("FeedbackSlider");
        }

        [HttpPost]
        public async Task<JsonResult> EditFeedbackSlide(UsersFeedback m)
        {
            string msg;
            string stat;
            if (ModelState.IsValid)
            {
                db.Update(m);
                try
                {
                    await db.SaveChangesAsync();
                    msg = "تغییرات با موفقیت ذخیره شد";
                    stat = "success";
                }
                catch (DbUpdateException e)
                {
                    msg = "خطا در ذخیره اطلاعات و برقراری ارتباط با پایگاه داده رخ داده است. لطفا مجدد تلاش کنید. جزئیات: " + e.Message;
                    stat = "danger";
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    stat = "danger";
                }
            }
            else
            {
                msg = "خطا در اعتبار سنجی فرم وجود دارد. لطفا فرم را بررسی و سپس تلاش کنید.";
                stat = "danger";
            }
            return Json(new { message = msg, status = stat });
        }

        [HttpPost]
        public async Task<JsonResult> EditFeedbackSlideImage(IFormFile image, int id)
        {
            string stat;
            string msg;
            if (image != null)
            {
                if (image.Length <= 512000)
                {
                    if (image.ContentType == "image/jpeg" || image.ContentType == "image/png" || image.ContentType == "image/webp")
                    {
                        var path = Path.Combine(_environment.WebRootPath, "img\\slider");
                        var i = await db.UsersFeedbacks.FindAsync(id);
                        if (System.IO.File.Exists(path + i.Picture))
                        {
                            try
                            {
                                System.IO.File.Delete(path + i.Picture);
                            }
                            catch (IOException e)
                            {
                                msg = "خطا! فایل تصویری در حال استفاده در یک پردازش دیگر است. لطفا دوباره امتحان کنید. جزئیات: " + e.Message;
                                stat = "uk-alert-danger";
                                return Json(new { message = msg, status = stat });
                            }
                        }

                        var fileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName).ToLower();
                        var P = Path.Combine(_environment.WebRootPath, "img\\slider", fileName);
                        using (var stream = new FileStream(P, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        try
                        {
                            i.Picture = fileName;
                            db.Update(i);
                            await db.SaveChangesAsync();
                            msg = "تصویر با موفقیت آپلود و جایگزین شد";
                            stat = "success";
                        }
                        catch (NotImplementedException notImp)
                        {
                            msg = notImp.Message;
                            stat = "danger";
                        }
                    }
                    else
                    {
                        msg = "لطفا از فرمت jpg  یا png استفاده کنید";
                        stat = "danger";
                    }
                }
                else
                {
                    msg = "حجم تصویر بیشتر از 512 کیلوبایت است";
                    stat = "danger";
                }
            }
            else
            {
                msg = "لطفا یک تصویر انتخاب کنید.";
                stat = "danger";
            }
            return Json(new
            {
                message = msg,
                status = stat
            });
        }

        public async Task<JsonResult> RemoveFeedbackSlide(int id)
        {
            string msg;
            string stat;
            var i = await db.UsersFeedbacks.FindAsync(id);
            if (i != null)
            {
                db.Remove(i);
                try
                {
                    await db.SaveChangesAsync();
                    msg = "عملیات حذف موفقیت آمیز بود.";
                    stat = "success";
                }
                catch (DbUpdateException e)
                {
                    msg = "خطا در ذخیره اطلاعات و برقراری ارتباط با پایگاه داده رخ داده است. لطفا مجدد تلاش کنید. جزئیات: " + e.Message;
                    stat = "danger";
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    stat = "danger";
                }
                //deleting from disk
                var path = Path.Combine(_environment.WebRootPath, "img\\slider", i.Picture);
                FileInfo fi = new FileInfo(path);
                try
                {
                    fi.Delete();
                }
                catch (IOException e)
                {
                    msg = e.Message;
                    stat = "danger";
                }
            }
            else
            {
                msg = "مورد درخواستی یافت نشد";
                stat = "danger";
            }
            return Json(new { message = msg, status = stat });
        }

        #endregion
    }
}
