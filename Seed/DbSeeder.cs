using APIWithRBACJWT.Models;
using Microsoft.AspNetCore.Identity;

namespace APIWithRBACJWT.Seed
{
	public static class DbSeeder
	{
		public static async Task SeedAsync(IServiceProvider services)
		{
			var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

			// Seed Roles
			foreach (var role in new[] { "Admin", "User" })
			{
				if (!await roleManager.RoleExistsAsync(role))
					await roleManager.CreateAsync(new IdentityRole(role));
			}

			// Seed Admin Account
			var adminEmail = "admin@example.com";
			var adminPassword = "Admin@123";
			var adminName = "System Admin";

			if (await userManager.FindByEmailAsync(adminEmail) is null)
			{
				var admin = new ApplicationUser
				{
					UserName = adminEmail,
					Email = adminEmail,
					FullName = adminName
				};

				var result = await userManager.CreateAsync(admin, adminPassword);
				if (result.Succeeded)
					await userManager.AddToRoleAsync(admin, "Admin");
			}
		}

	}
}
