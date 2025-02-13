using ElMagzer.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace ElMagzer.Core.Services
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(AppUser user,UserManager<AppUser> userManager);
    }
}
