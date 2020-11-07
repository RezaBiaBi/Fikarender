using Microsoft.AspNetCore.Identity;
using System;
using Fikarender.Data;

namespace Helpers
{
    public static class AdminUserAndRoleInit
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByNameAsync("09101234567").Result == null)
            {
                var user = new ApplicationUser()
                {
                    Firstname = "ادمین",
                    Lastname = "سیستم",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "09101234567",
                    PhoneNumberConfirmed = true,
                    RegisterDate = DateTime.Now,
                    UserName = "09101234567"
                };

                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rd1!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "User").Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Admin"
                };

                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
