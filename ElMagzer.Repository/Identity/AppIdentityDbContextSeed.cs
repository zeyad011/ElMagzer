using ElMagzer.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (userManager.Users.Count() == 0)
            {
                var user = new AppUser()
                {
                    DisplayName = "Ahmed",
                    Email = "ahmed.test@gmail.com",
                    UserName = "Ahmed",
                    PhoneNumber = "01150857148"
                };

                await userManager.CreateAsync(user,"1234");
            }
        }
    }
}
