using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public static class SeedAdmin
{
    public static async Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // إنشاء دور Admin لو مش موجود
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        // إنشاء مستخدم Admin لو مش موجود
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
