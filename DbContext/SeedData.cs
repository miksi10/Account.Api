using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Account.Api.DbContext
{
    public class SeedData
    {
        public static void EnsureSeedData(WebApplication app)
        {
            try
            {
                using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<AuthDbContext>();

                    if (context != null)
                    {
                        //context.Database.EnsureDeleted();
                        context.Database.Migrate();
                    }

                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    //some test data
                    //if already exists, code will go to catch block because test data is already inserted
                    //Api will be started normally, even if that happens
                    if (roleManager != null)
                    {
                        var userRole = roleManager.CreateAsync(new IdentityRole("User")).Result;

                        var gameMasterRole = roleManager.CreateAsync(new IdentityRole("GameMaster")).Result;
                    }

                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                    if (userManager != null)
                    {
                        var firstUser = new IdentityUser("firstUser");
                        var firstUserCreate = userManager.CreateAsync(firstUser, "Pass123#").Result;

                        if (firstUserCreate.Succeeded)
                        {
                            var firstUserRole = userManager.AddToRoleAsync(firstUser, "User").Result;
                        }

                        var firstGameMaster = new IdentityUser("firstGameMaster");
                        var firstGameMasterCreate = userManager.CreateAsync(firstGameMaster, "Pass123#").Result;

                        if (firstGameMasterCreate.Succeeded)
                        {
                            var firstGameMasterRole = userManager.AddToRoleAsync(firstGameMaster, "GameMaster").Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
