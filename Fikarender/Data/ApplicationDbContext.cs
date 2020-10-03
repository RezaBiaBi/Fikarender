using System.Data;
using Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fikarender.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AdminTheme> AdminTheme { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Assist> Assists { get; set; }  
        public DbSet<WorkSample> WorkSamples { get; set; }  
        public DbSet<Tag> Tag { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<BlogTag> BlogTag { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<FaqTag> FaqTags { get; set; }  

        /*public DbSet<Config> Config { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<MainSlider> MainSlider { get; set; }*/

        public DbSet<ShortLink> ShortLink { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<UsersFeedback> UsersFeedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("dbo");

           /* modelBuilder.Entity<Config>().HasData(new Config { Id = 1, OrderNumber = 10000, ShippingPrice = 25000, FreeShippingLimit = 500000, HomeActiveSliderCategoryId = 1 });*/
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                builder.UseSqlServer(ConfigurationManager.AppSetting["ConnectionStrings:DefaultConnection"]);

                /*builder.UseLazyLoadingProxies()
                    .UseSqlServer(ConfigurationManager.AppSetting["ConnectionStrings:DefaultConnection"]);

                builder.UseApplicationServiceProvider(new DataTable("tableName", "tableNameSpace"));*/
            }

            base.OnConfiguring(builder);
        }
    }
}
