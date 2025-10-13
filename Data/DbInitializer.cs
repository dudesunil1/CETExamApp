using CETExamApp.Models;
using Microsoft.AspNetCore.Identity;

namespace CETExamApp.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Create roles
            string[] roleNames = { "Admin", "Student" };
            
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create default admin user
            var adminEmail = "admin@cetexam.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

            // Create default student user
            var studentEmail = "student@cetexam.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);

            if (studentUser == null)
            {
                var newStudent = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FirstName = "Test",
                    LastName = "Student",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(newStudent, "Student@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newStudent, "Student");
                }
            }
        }
    }
}

