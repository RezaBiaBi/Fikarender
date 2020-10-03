using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Fikarender.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Helpers;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity.UI.Services;
using reCAPTCHA.AspNetCore;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using WebMarkupMin.AspNetCore3;

namespace Fikarender
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 52428800;
                /*options.AllowSynchronousIO = true;
                options.AuthenticationDisplayName = "Auth_displayeName";
                options.AutomaticAuthentication = false;*/
            });
            services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 52428800; });

            services.AddHttpClient();

            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseLazyLoadingProxies()
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            #region External Authentication(Google)

            /*services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("ExternalLoginProviders:GoogleProvider");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.CallbackPath = new PathString($"/Account/signin-google");
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                });*/

            #endregion

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters = "0123456789";
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = true;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultPhoneProvider;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddRouting(option =>
            {
                option.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
                option.LowercaseUrls = true;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Account/Login";
                options.LogoutPath = $"/Account/Logout";
                options.AccessDeniedPath = $"/Account/AccessDenied";
            });

            #region Inversion Of Controll(IoC)

            services.AddSession();
            services.AddSingleton(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.Arabic }));
            services.AddSingleton<ILinkTools, LinkTools>();
            services.AddRecaptcha(Configuration.GetSection("RecaptchaSettings"));
            services.AddScoped<IEmailSender, EmailSender>();

            #endregion

            #region markup Web Html Compression

            services.AddWebMarkupMin(options =>
                {
                    options.AllowCompressionInDevelopmentEnvironment = true;
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    #region more Options

                    /*options.IsCompressionEnabled();
                    options.IsMinificationEnabled();
                    options.IsAllowableResponseSize(Int64.MaxValue);
                    options.IsPoweredByHttpHeadersEnabled();*/

                    #endregion
                }).AddHtmlMinification().AddHttpCompression()
                .AddXhtmlMinification().AddXmlMinification();

            #endregion

            #region Add MemoryCache

            /*services.AddMemoryCache(options =>
            {
                *//*options.CompactionPercentage = Double.Epsilon;
                options.ExpirationScanFrequency = TimeSpan.FromDays(1);
                options.SizeLimit = Int64.MaxValue;*/
                /*options.Clock = new SystemClock();*//*
            });

            services.AddDistributedMemoryCache(options =>
            {
                *//*options.CompactionPercentage = Double.Epsilon;
                options.ExpirationScanFrequency = TimeSpan.FromDays(1);
                options.SizeLimit = Int64.MaxValue;*/
                /*options.Clock = new SystemClock();*//*
            });*/

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseDatabaseErrorPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            #region Create Default page html (Intro WebPage)

            /*var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("IntroIndex.html");
            app.UseDefaultFiles(options);*/

            #endregion

            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();

            #region Page for handle status code

            /*app.UseStatusCodePages(subApp =>
            {
                subApp.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<p>html supported type content!!! so create page for status code handle</p>");
                    context.Response.Redirect("/Home/StatusError");//TODO Create StatusCode Page
                });
            });*/

            #endregion
            
            app.UseRewriter(new RewriteOptions().Add(new RedirectLowerCaseRule()));
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            #region Use Google Authentication

            /*app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                ClientId = "YOUR_CLIENT_ID",
                ClientSecret = "YOUR_CLIENT_SECRET",
                CallbackPath = new PathString("/signin-google"),
                SignInScheme = "MainCookie"
            });*/

            #endregion

            app.UseSession();
            /*app.Use(async (context, next) =>
            {
                if (context.Session != null)
                {
                    var u = context.User;
                    if (u.Identity.IsAuthenticated)
                    {
                        string userName = context.User.Identity.Name;
                        if (context.Session.GetString("Name") == null || context.Session.GetString("Avatar") == null)
                        {
                            using var db = new ApplicationDbContext();
                            var user = await db.ApplicationUser.AsNoTracking().SingleOrDefaultAsync(a => a.UserName.Equals(userName));
                            context.Session.SetString("Name", $"{user.Firstname} {user.Lastname}");
                            context.Session.SetString("Avatar", string.IsNullOrEmpty(user.Avatar) ? "" : user.Avatar);
                        }
                    }
                    else
                    {
                        if (context.Session.Get("Name") != null)
                            context.Session.Remove("Name");

                        if (context.Session.Get("Avatar") != null)
                            context.Session.Remove("Avatar");
                    }
                }
                await next.Invoke();
            });*/

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller:slugify}/{action:slugify}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
                endpoints.MapRazorPages();
            });
        }
    }
}
