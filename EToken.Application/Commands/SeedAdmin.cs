// Example in a seeder or admin creation endpoint:
using EToken.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EToken.Application.Commands;

public class AdminSeedHandler(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
{
    public async Task InitAdmin()
    {
        // 1. Create the role if it doesn't exist
if (!await roleManager.RoleExistsAsync("Admin"))
{
    await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
}

// 2. Assign the role to the user
var user = await userManager.FindByNameAsync("muhammadolammi");
      
if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
{
    await userManager.AddToRoleAsync(user, "Admin");
}

    }
}